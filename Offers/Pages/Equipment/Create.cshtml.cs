using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

    [BindProperty]
    public string StatusMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if(string.IsNullOrEmpty(Equipment.Name))
        {
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

        StatusMessage = "Equipment created successfully.";

        return RedirectToPage("./List");
    }
}
