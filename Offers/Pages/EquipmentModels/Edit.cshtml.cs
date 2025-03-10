using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Pages.EquipmentModelPage
{
    [Authorize(Roles = "Admin, User")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EquipmentModel EquipmentModel { get; set; }

        [BindProperty]
        public List<EquipmentModelFeature> Features { get; set; }

        public List<Unit> Units { get; set; }
        public SelectList EquipmentList { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            EquipmentModel = await _context.EquipmentModels
                .Include(e => e.Equipment)
                .Include(e => e.Features)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (EquipmentModel == null)
            {
                return NotFound();
            }

            Features = EquipmentModel.Features.ToList();
            await LoadEquipmentList();
            Units = await _context.Units.OrderBy(u => u.Name).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (EquipmentModel.EquipmentId == null || string.IsNullOrEmpty(EquipmentModel.Brand) || string.IsNullOrEmpty(EquipmentModel.Model))
            {
                await LoadEquipmentList();
                Units = await _context.Units.OrderBy(u => u.Name).ToListAsync();
                return Page();
            }

            var existingFeatures = await _context.EquipmentModelFeatures
            .Where(f => f.EquipmentModelId == EquipmentModel.Id)
            .ToListAsync();

            _context.EquipmentModelFeatures.RemoveRange(existingFeatures);

            foreach (var feature in Features)
            {
                feature.EquipmentModelId = EquipmentModel.Id;
                _context.EquipmentModelFeatures.Add(feature);
            }

            _context.Attach(EquipmentModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EquipmentModelExists(EquipmentModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    StatusMessage = "Error: Güncelleme sýrasýnda bir hata oluþtu.";
                    await LoadEquipmentList();
                    Units = await _context.Units.OrderBy(u => u.Name).ToListAsync();
                    return Page();
                }
            }

            return RedirectToPage("./Index");
        }

        private async Task LoadEquipmentList()
        {
            EquipmentList = new SelectList(
                await _context.Equipment.OrderBy(e => e.Name).ToListAsync(),
                "Id",
                "Name"
            );
        }

        private bool EquipmentModelExists(int id)
        {
            return _context.EquipmentModels.Any(e => e.Id == id);
        }
    }
} 