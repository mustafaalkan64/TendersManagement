using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pages.EquipmentModelPage
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<EquipmentModel> EquipmentModels { get; set; }
        public List<Company> Companies { get; set; }

        public async Task OnGetAsync()
        {
            EquipmentModels = await _context.EquipmentModels
            .Include(em => em.Equipment)
            .Include(em => em.CompanyEquipmentModels)
                .ThenInclude(cem => cem.Company)
            .OrderBy(em => em.Equipment.Name)
            .ThenBy(em => em.Brand)
            .ThenBy(em => em.Model)
            .ToListAsync();

            Companies = await _context.Companies
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
        public async Task<IActionResult> OnPostAddCompanyAsync([FromBody] CompanyAssignmentModel model)
        {
            var exists = await _context.CompanyEquipmentModels
                .AnyAsync(cem => cem.CompanyId == model.CompanyId &&
                                cem.EquipmentModelId == model.EquipmentModelId);

            if (!exists)
            {
                _context.CompanyEquipmentModels.Add(new CompanyEquipmentModel
                {
                    CompanyId = model.CompanyId,
                    EquipmentModelId = model.EquipmentModelId
                });
                await _context.SaveChangesAsync();
            }

            return new JsonResult(new { success = true });
        }

        public async Task<IActionResult> OnPostRemoveCompanyAsync([FromBody] CompanyAssignmentModel model)
        {
            var companyEquipmentModel = await _context.CompanyEquipmentModels
                .FirstOrDefaultAsync(cem => cem.CompanyId == model.CompanyId &&
                                          cem.EquipmentModelId == model.EquipmentModelId);

            if (companyEquipmentModel != null)
            {
                _context.CompanyEquipmentModels.Remove(companyEquipmentModel);
                await _context.SaveChangesAsync();
            }

            return new JsonResult(new { success = true });
        }
    }

    public class CompanyAssignmentModel
    {
        public int EquipmentModelId { get; set; }
        public int CompanyId { get; set; }
    }
} 