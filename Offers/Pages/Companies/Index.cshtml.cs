// All common using statements are now in GlobalUsings.cs
using Offers.Services.Company;

namespace Offers.Pages.Companies
{
    [Authorize(Policy = "CanListCompany")]
    public class IndexModel : PageModel
    {
        private readonly ICompanyService _companyService;

        public IndexModel(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        public IList<Company> Companies { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Companies = await _companyService.GetCompaniesAsync();
        }
    }
} 