using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pages.Offers
{
    [Authorize(Policy = "CanListOffer")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public List<Offer> Offers { get; set; }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            var query = _context.Offers
                .AsNoTracking()
                .Include(a => a.ProjectOwner)
                .Include(o => o.OfferItems)
                    .ThenInclude(x => x.Company)
                .AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
            {
                var searchTerm = SearchString;
                query = query.Where(em =>
                    em.OfferName.ToLower().Contains(searchTerm.ToLower())
                    || em.ProjectOwner.Name.ToLower().Contains(searchTerm.ToLower()));
            }
            Offers = await query
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            foreach (var Offer in Offers)
            {
                Offer.TotalPrice = 0;
                if(Offer.OfferItems.Any())
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
} 