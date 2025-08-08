// All common using statements are now in GlobalUsings.cs

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
            var query = _context.EquipmentModels
                .Include(em => em.Equipment)
                .Include(em => em.CompanyEquipmentModels)
                    .ThenInclude(cem => cem.Company)
                .AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(em =>
                    em.Equipment.Name.Contains(SearchString, StringComparison.OrdinalIgnoreCase) ||
                    em.Brand.Contains(SearchString, StringComparison.OrdinalIgnoreCase) ||
                    em.Capacity.Contains(SearchString, StringComparison.OrdinalIgnoreCase) ||
                    em.Model.Contains(SearchString, StringComparison.OrdinalIgnoreCase));
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