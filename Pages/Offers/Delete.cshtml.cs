using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Offer Offer { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Offer = await _context.Offers
            .Include(o => o.OfferItems)
                .ThenInclude(oi => oi.Equipment)
            .Include(o => o.OfferItems)
                .ThenInclude(oi => oi.Company)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (Offer == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Offer = await _context.Offers.FindAsync(id);

        if (Offer != null)
        {
            _context.Offers.Remove(Offer);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
} 