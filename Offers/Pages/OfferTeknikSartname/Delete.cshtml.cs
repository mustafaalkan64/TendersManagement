using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Pages.OfferTeknikSartname
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.OfferTeknikSartname OfferTeknikSartname { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            OfferTeknikSartname = await _context.OfferTeknikSartnames.FindAsync(id);

            if (OfferTeknikSartname == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var offerId = OfferTeknikSartname.OfferId;
            if (OfferTeknikSartname == null)
            {
                return NotFound();
            }

            _context.OfferTeknikSartnames.Remove(OfferTeknikSartname);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { offerId = offerId });
        }
    }
}
