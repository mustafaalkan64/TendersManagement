using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;

namespace Pages.OfferTeknikSartname
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.OfferTeknikSartname OfferTeknikSartname { get; set; }

        public void OnGet(int offerId)
        {
            OfferTeknikSartname = new Models.OfferTeknikSartname { OfferId = offerId };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("OfferTeknikSartname.Offer"); // Ignore Customer validation
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.OfferTeknikSartnames.Add(OfferTeknikSartname);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { offerId = OfferTeknikSartname.OfferId });
        }
    }
}
