using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Equipment Equipment { get; set; }

    [BindProperty]
    public List<EquipmentFeature> Features { get; set; } = new List<EquipmentFeature>();

    public List<Unit> Units { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Units = await _context.Units.OrderBy(u => u.Name).ToListAsync();
        return Page();
    }
    [BindProperty]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(Equipment.Name))
            {
                Units = await _context.Units.OrderBy(u => u.Name).ToListAsync();
                StatusMessage = "Ekipman adý boþ olamaz";
                return Page();
            }

            _context.Equipment.Add(Equipment);
            await _context.SaveChangesAsync();

            // Add features
            foreach (var feature in Features)
            {
                feature.EquipmentId = Equipment.Id;
                _context.EquipmentFeatures.Add(feature);
            }
            await _context.SaveChangesAsync();

            return RedirectToPage("./Edit", new { id = Equipment.Id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Ekipman kaydedilirken bir hata oluþtu.");
            Units = await _context.Units.OrderBy(u => u.Name).ToListAsync();
            return Page();
        }
    }
}
