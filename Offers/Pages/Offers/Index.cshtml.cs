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
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            foreach (var Offer in Offers)
            {
                Offer.TotalPrice = Offer.OfferItems.Sum(item => item.Price * item.Quantity);
            }
        }
    }
} 