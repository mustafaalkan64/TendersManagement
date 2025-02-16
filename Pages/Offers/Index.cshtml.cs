using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
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

        public async Task OnGetAsync()
        {
            Offers = await _context.Offers
                .Include(o => o.OfferItems)
                .OrderByDescending(o => o.CreatedDate)
                .ToListAsync();
        }
    }
} 