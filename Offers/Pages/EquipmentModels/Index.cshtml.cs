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
    [Authorize(Policy = "CanListEquipmentModel")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<EquipmentModel> EquipmentModels { get; set; }
        public List<Company> Companies { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public async Task OnGetAsync(CancellationToken cancellationToken = default)
        {
            //EquipmentModels = await _context.EquipmentModels
            //.Include(em => em.Equipment)
            //.Include(em => em.CompanyEquipmentModels)
            //    .ThenInclude(cem => cem.Company)
            //.OrderBy(em => em.Equipment.Name)
            //.ThenBy(em => em.Brand)
            //.ThenBy(em => em.Model)
            //.ToListAsync();

            var query = _context.EquipmentModels
            .Include(em => em.Equipment)
            .Include(em => em.CompanyEquipmentModels)
                .ThenInclude(cem => cem.Company)
            .AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
            {
                var searchTerm = SearchString.ToLower();
                query = query.Where(em =>
                    em.Equipment.Name.ToLower().Contains(SearchString.ToLower()) ||
                    em.Brand.ToLower().Contains(SearchString.ToLower()) ||
                    em.Capacity.ToLower().Contains(SearchString.ToLower()) ||
                    em.Model.ToLower().Contains(SearchString.ToLower()));
            }

            EquipmentModels = await query
                .OrderBy(em => em.Equipment.Name)
                .ThenBy(em => em.Brand)
                .ThenBy(em => em.Model)
                .ToListAsync(cancellationToken);

            Companies = await _context.Companies
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }
        public async Task<IActionResult> OnPostAddCompanyAsync([FromBody] CompanyAssignmentModel model, CancellationToken cancellationToken = default)
        {
            var exists = await _context.CompanyEquipmentModels
                .AnyAsync(cem => cem.CompanyId == model.CompanyId &&
                                cem.EquipmentModelId == model.EquipmentModelId, cancellationToken);

            if (!exists)
            {
                _context.CompanyEquipmentModels.Add(new CompanyEquipmentModel
                {
                    CompanyId = model.CompanyId,
                    EquipmentModelId = model.EquipmentModelId
                });
                await _context.SaveChangesAsync(cancellationToken);
            }

            return new JsonResult(new { success = true });
        }

        public async Task<IActionResult> OnPostRemoveCompanyAsync([FromBody] CompanyAssignmentModel model, CancellationToken cancellationToken = default)
        {
            var companyEquipmentModel = await _context.CompanyEquipmentModels
                .FirstOrDefaultAsync(cem => cem.CompanyId == model.CompanyId &&
                                          cem.EquipmentModelId == model.EquipmentModelId, cancellationToken);

            if (companyEquipmentModel != null)
            {
                _context.CompanyEquipmentModels.Remove(companyEquipmentModel);
                await _context.SaveChangesAsync(cancellationToken);
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