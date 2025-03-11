using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Pages.OfferTeknikSartname
{
    [Authorize(Policy = "CanEditTeknikSartname")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
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
            ModelState.Remove("OfferTeknikSartname.Offer"); // Ignore Customer validation
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(OfferTeknikSartname).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OfferTeknikSartnameExists(OfferTeknikSartname.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index", new { offerId = OfferTeknikSartname.OfferId });
        }

        private bool OfferTeknikSartnameExists(int id)
        {
            return _context.OfferTeknikSartnames.Any(e => e.Id == id);
        }
    }
}
