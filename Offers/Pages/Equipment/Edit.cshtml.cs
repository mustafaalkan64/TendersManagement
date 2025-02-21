using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Offers.Pages.Equipment
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<EquipmentFeature> Features { get; set; }

        [BindProperty]
        public Models.Equipment Equipment { get; set; }

        public List<Unit> Units { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Equipment = await _context.Equipment
                .Include(e => e.Features)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Equipment == null)
            {
                return NotFound();
            }

            Features = Equipment.Features.ToList();
            Units = await _context.Units.OrderBy(u => u.Name).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Remove existing features
                var existingFeatures = await _context.EquipmentFeatures
                    .Where(f => f.EquipmentId == Equipment.Id)
                    .ToListAsync();
                _context.EquipmentFeatures.RemoveRange(existingFeatures);

                // Add new features
                foreach (var feature in Features)
                {
                    feature.EquipmentId = Equipment.Id;
                    _context.EquipmentFeatures.Add(feature);
                }
                _context.Attach(Equipment).State = EntityState.Modified;

                StatusMessage = "Equipment updated successfully.";
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EquipmentExists(Equipment.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./List");
        }

        private bool EquipmentExists(int id)
        {
            return _context.Equipment.Any(e => e.Id == id);
        }
    }
}
