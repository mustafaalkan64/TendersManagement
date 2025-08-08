// All common using statements are now in GlobalUsings.cs

using Offers.Services.EquipmentModel;

namespace Pages.EquipmentModelPage
{
    [Authorize(Policy = "CanListEquipmentModel")]
    public class IndexModel : PageModel
    {
        private readonly IEquipmentModelService _equipmentModelService;

        public IndexModel(IEquipmentModelService equipmentModelService)
        {
            _equipmentModelService = equipmentModelService;
        }

        public IList<EquipmentModel> EquipmentModels { get; set; }
        public List<Company> Companies { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public async Task OnGetAsync(CancellationToken cancellationToken = default)
        {
            EquipmentModels = await _equipmentModelService.GetEquipmentModelsAsync(SearchString, cancellationToken);
            Companies = await _equipmentModelService.GetCompaniesAsync(cancellationToken);
        }
        public async Task<IActionResult> OnPostAddCompanyAsync([FromBody] CompanyAssignmentModel model, CancellationToken cancellationToken = default)
        {
            await _equipmentModelService.AddCompanyAssignmentAsync(model.CompanyId, model.EquipmentModelId, cancellationToken);
            return new JsonResult(new { success = true });
        }

        public async Task<IActionResult> OnPostRemoveCompanyAsync([FromBody] CompanyAssignmentModel model, CancellationToken cancellationToken = default)
        {
            await _equipmentModelService.RemoveCompanyAssignmentAsync(model.CompanyId, model.EquipmentModelId, cancellationToken);
            return new JsonResult(new { success = true });
        }
    }

    public class CompanyAssignmentModel
    {
        public int EquipmentModelId { get; set; }
        public int CompanyId { get; set; }
    }
} 