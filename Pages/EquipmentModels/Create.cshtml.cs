using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;

namespace EquipmentModels.Pages
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IAntiforgery _antiforgery;

        public CreateModel(ApplicationDbContext context, IAntiforgery antiforgery)
        {
            _context = context;
            _antiforgery = antiforgery;
        }

        [BindProperty]
        public EquipmentModel EquipmentModel { get; set; }

        [BindProperty]
        public List<EquipmentModelFeature> Features { get; set; } = new List<EquipmentModelFeature>();

        public SelectList EquipmentList { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadEquipmentList();
            return Page();
        }

        public async Task<IActionResult> OnGetEquipmentFeaturesAsync(int equipmentId)
        {
            var features = await _context.EquipmentFeatures
                .Where(ef => ef.EquipmentId == equipmentId)
                .Select(ef => new
                {
                    featureKey = ef.FeatureKey,
                    featureValue = ef.FeatureValue
                })
                .ToListAsync();

            return new JsonResult(features);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                StatusMessage = "Error: Please correct the errors below.";
                await LoadEquipmentList();
                return Page();
            }

            try
            {
                _context.EquipmentModels.Add(EquipmentModel);
                await _context.SaveChangesAsync();

                // Add features
                foreach (var feature in Features)
                {
                    feature.EquipmentModelId = EquipmentModel.Id;
                    _context.EquipmentModelFeatures.Add(feature);
                }
                await _context.SaveChangesAsync();

                StatusMessage = "Equipment model created successfully.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, 
                    "An error occurred while saving the equipment model. Please try again.");
                StatusMessage = "Error: Failed to create equipment model.";
                await LoadEquipmentList();
                return Page();
            }
        }

        private async Task LoadEquipmentList()
        {
            EquipmentList = new SelectList(
                await _context.Equipment.OrderBy(e => e.Name).ToListAsync(),
                "Id",
                "Name"
            );
        }
    }
} 