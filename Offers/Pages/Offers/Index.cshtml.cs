using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;

namespace Pages.Offers
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Offer> Offers { get; set; }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            Offers = await _context.Offers
                .AsNoTracking()
                .Include(o => o.OfferItems)
                    .ThenInclude(x => x.Company)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            foreach (var Offer in Offers)
            {
                Offer.TotalPrice = Offer.OfferItems
                    .GroupBy(oi => oi.Company.Name)
                    .Select(g => new CompanySummaryViewModel
                    {
                        CompanyName = g.Key,
                        TotalPrice = g.Sum(oi => oi.Price * oi.Quantity)
                    })
                    .OrderBy(s => s.TotalPrice).First().TotalPrice;
            }
        }
    }
} 