using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;
namespace Offers.Pages.Companies
{
    [Authorize(Policy = "CanEditCompany")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
            SelectedEquipmentModels = new List<CompanyEquipmentModel>();
        }

        [BindProperty]
        public Company Company { get; set; }
        public List<CompanyEquipmentModel> SelectedEquipmentModels { get; set; }
        
        [BindProperty]
        public int NewEquipmentModelId { get; set; }

        public SelectList EquipmentModelList { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Company = await _context.Companies
                .Include(x => x.CompanyEquipmentModels)
                .ThenInclude(x => x.EquipmentModel).ThenInclude(x => x.Equipment)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Company == null)
            {
                return NotFound();
            }

            // Initialize session with existing equipment models
            SelectedEquipmentModels = Company.CompanyEquipmentModels
                .ToList();
            HttpContext.Session.Set("SelectedEquipmentModels", SelectedEquipmentModels);

            return Page();
        }

        public async Task<IActionResult> OnPostUpdatePriceAsync(int id, decimal price)
        {
            var companyEquipmentModel = await _context.CompanyEquipmentModels
                .FirstOrDefaultAsync(x => x.Id == id);

            if (companyEquipmentModel == null)
            {
                return NotFound();
            }

            companyEquipmentModel.Price = price;
            _context.Attach(companyEquipmentModel).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true });
        }


        public async Task<IActionResult> OnGetEquipmentModelPriceAsync(int companyId, int equipmentModelId)
        {
            var companyEquipmentModel = await _context.CompanyEquipmentModels
                .Where(cem => cem.CompanyId == companyId && cem.EquipmentModelId == equipmentModelId)
                .Select(cem => new { cem.Price })
                .FirstOrDefaultAsync();

            return new JsonResult(new { price = companyEquipmentModel?.Price ?? 0 });
        }

        public async Task<IActionResult> OnPostAddEquipmentModelAsync()
        {
            if (NewEquipmentModelId == 0)
            {
                ModelState.AddModelError("NewEquipmentModelId", "L�tfen ekipman model se�iniz");
                return Page();
            }

            var equipmentModel = await _context.CompanyEquipmentModels
                .Include(x => x.EquipmentModel)
                 .ThenInclude(em => em.Equipment)
                .FirstOrDefaultAsync(em => em.Id == NewEquipmentModelId);

            if (equipmentModel != null)
            {
                SelectedEquipmentModels = HttpContext.Session.Get<List<CompanyEquipmentModel>>("SelectedEquipmentModels") ?? new List<CompanyEquipmentModel>();

                foreach (var selectedEquipmentModel in SelectedEquipmentModels)
                {
                    selectedEquipmentModel.EquipmentModel.Equipment = await _context.Equipment.AsNoTracking().FirstOrDefaultAsync(x => x.Id == selectedEquipmentModel.EquipmentModel.EquipmentId);
                }
                if (!SelectedEquipmentModels.Any(em => em.Id == equipmentModel.Id))
                {
                    SelectedEquipmentModels.Add(equipmentModel);
                    HttpContext.Session.Set("SelectedEquipmentModels", SelectedEquipmentModels);
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostRemoveEquipmentModelAsync(int equipmentModelId, CancellationToken cancellationToken = default)
        {
            SelectedEquipmentModels = HttpContext.Session.Get<List<CompanyEquipmentModel>>("SelectedEquipmentModels") ?? new List<CompanyEquipmentModel>();
            foreach (var selectedEquipmentModel in SelectedEquipmentModels)
            {
                selectedEquipmentModel.EquipmentModel.Equipment = await _context.Equipment.AsNoTracking().FirstOrDefaultAsync(x => x.Id == selectedEquipmentModel.EquipmentModel.EquipmentId, cancellationToken);
            }
            var itemToRemove = SelectedEquipmentModels.FirstOrDefault(em => em.Id == equipmentModelId);

            if (itemToRemove != null)
            {
                SelectedEquipmentModels.Remove(itemToRemove);
                _context.CompanyEquipmentModels.Remove(itemToRemove);
                await _context.SaveChangesAsync(cancellationToken);
                HttpContext.Session.Set("SelectedEquipmentModels", SelectedEquipmentModels);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSaveAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            // Remove existing equipment models
            var existingModels = await _context.CompanyEquipmentModels
                .Where(cem => cem.CompanyId == Company.Id)
                .ToListAsync();

            _context.CompanyEquipmentModels.RemoveRange(existingModels);

            // Add selected equipment models
            var selectedModels = HttpContext.Session.Get<List<CompanyEquipmentModel>>("SelectedEquipmentModels") ?? new List<CompanyEquipmentModel>();
            foreach (var model in selectedModels)
            {
                var companyEquipmentModel = new CompanyEquipmentModel
                {
                    CompanyId = model.CompanyId,
                    EquipmentModelId = model.EquipmentModelId,
                    Price = model.Price
                };
                _context.CompanyEquipmentModels.Add(companyEquipmentModel);
            }


            // Clear session
            HttpContext.Session.Remove("SelectedEquipmentModels");

            _context.Attach(Company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(Company.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
    }
}