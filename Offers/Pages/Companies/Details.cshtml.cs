using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models;
namespace Offers.Pages.Companies
{
    [Authorize(Policy = "CanSeeDetailsCompany")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Company Company { get; set; }
        public List<Offer> CompanyOffers { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Company == null)
            {
                return NotFound();
            }

            // Get the last 10 offers for this company
            CompanyOffers = await _context.Offers
                .Include(o => o.OfferItems)
                .Where(o => o.OfferItems.Any(oi => oi.CompanyId == id))
                .OrderByDescending(o => o.CreatedDate)
                .Take(10)
                .ToListAsync();

            return Page();
        }
    }
} 