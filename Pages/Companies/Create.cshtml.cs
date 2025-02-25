using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApp.Pages.Companies
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
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

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadEquipmentModelList();
            SelectedEquipmentModels = HttpContext.Session.Get<List<EquipmentModel>>("SelectedEquipmentModels") ?? new List<EquipmentModel>();
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

            _context.Companies.Add(Company);
            await _context.SaveChangesAsync();

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
            await _context.SaveChangesAsync();

            // Clear session
            HttpContext.Session.Remove("SelectedEquipmentModels");

            return RedirectToPage("./Index");
        }
    }
} 