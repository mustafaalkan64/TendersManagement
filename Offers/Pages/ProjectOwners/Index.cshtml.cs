using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Offers.Services.ProjectOwner;
using Models;

namespace Pages.ProjectOwner
{
    [Authorize(Policy = "CanListOwner")]
    public class IndexModel : PageModel
    {
        private readonly IProjectOwnerService _projectOwnerService;

        public IndexModel(IProjectOwnerService projectOwnerService)
        {
            _projectOwnerService = projectOwnerService;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public IList<Models.ProjectOwner> ProjectOwners { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            ProjectOwners = await _projectOwnerService.GetProjectOwnersAsync(SearchString);
        }
    }
}