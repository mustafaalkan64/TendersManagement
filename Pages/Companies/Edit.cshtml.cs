using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Pages.Companies
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
            SelectedEquipmentModels = new List<EquipmentModel>();
        }

        [BindProperty]
        public Company Company { get; set; }

        [BindProperty]
        public int NewEquipmentModelId { get; set; }

        public SelectList EquipmentModelList { get; set; }
        public List<EquipmentModel> SelectedEquipmentModels { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Company = await _context.Companies
                .Include(c => c.CompanyEquipmentModels)
                    .ThenInclude(cem => cem.EquipmentModel)
                        .ThenInclude(em => em.Equipment)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (Company == null)
            {
                return NotFound();
            }

            // Initialize session with existing equipment models
            SelectedEquipmentModels = Company.CompanyEquipmentModels
                .Select(cem => cem.EquipmentModel)
                .ToList();
            HttpContext.Session.Set("SelectedEquipmentModels", SelectedEquipmentModels);

            await LoadEquipmentModelList();
            return Page();
        }

        private async Task LoadEquipmentModelList()
        {
            var equipmentModels = await _context.EquipmentModels
                .Include(em => em.Equipment)
                .OrderBy(em => em.Equipment.Name)
                .ThenBy(em => em.Brand)
                .ThenBy(em => em.Model)
                .ToListAsync();

            EquipmentModelList = new SelectList(equipmentModels, "Id", "FullName");
        }

        public async Task<IActionResult> OnPostAddEquipmentModelAsync()
        {
            if (NewEquipmentModelId == 0)
            {
                ModelState.AddModelError("NewEquipmentModelId", "Please select an equipment model");
                await LoadEquipmentModelList();
                return Page();
            }

            var equipmentModel = await _context.EquipmentModels
                .Include(em => em.Equipment)
                .FirstOrDefaultAsync(em => em.Id == NewEquipmentModelId);

            if (equipmentModel != null)
            {
                SelectedEquipmentModels = HttpContext.Session.Get<List<EquipmentModel>>("SelectedEquipmentModels") ?? new List<EquipmentModel>();
                
                if (!SelectedEquipmentModels.Any(em => em.Id == equipmentModel.Id))
                {
                    SelectedEquipmentModels.Add(equipmentModel);
                    HttpContext.Session.Set("SelectedEquipmentModels", SelectedEquipmentModels);
                }
            }

            await LoadEquipmentModelList();
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveEquipmentModelAsync(int equipmentModelId)
        {
            SelectedEquipmentModels = HttpContext.Session.Get<List<EquipmentModel>>("SelectedEquipmentModels") ?? new List<EquipmentModel>();
            var itemToRemove = SelectedEquipmentModels.FirstOrDefault(em => em.Id == equipmentModelId);
            
            if (itemToRemove != null)
            {
                SelectedEquipmentModels.Remove(itemToRemove);
                HttpContext.Session.Set("SelectedEquipmentModels", SelectedEquipmentModels);
            }

            await LoadEquipmentModelList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadEquipmentModelList();
                return Page();
            }

            // Remove existing equipment models
            var existingModels = await _context.CompanyEquipmentModels
                .Where(cem => cem.CompanyId == Company.Id)
                .ToListAsync();
            _context.CompanyEquipmentModels.RemoveRange(existingModels);

            // Add selected equipment models
            var selectedModels = HttpContext.Session.Get<List<EquipmentModel>>("SelectedEquipmentModels") ?? new List<EquipmentModel>();
            foreach (var equipmentModel in selectedModels)
            {
                var companyEquipmentModel = new CompanyEquipmentModel
                {
                    CompanyId = Company.Id,
                    EquipmentModelId = equipmentModel.Id
                };
                _context.CompanyEquipmentModels.Add(companyEquipmentModel);
            }

            _context.Attach(Company).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Clear session
            HttpContext.Session.Remove("SelectedEquipmentModels");

            return RedirectToPage("./Index");
        }
    }
} 