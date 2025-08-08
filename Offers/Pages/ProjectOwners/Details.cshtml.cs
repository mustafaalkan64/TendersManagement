using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Offers.Services.ProjectOwner;
using Models;

namespace Pages.ProjectOwner
{
    [Authorize(Policy = "CanSeeDetailsOwner")]
    public class DetailsModel : PageModel
    {
        private readonly IProjectOwnerService _projectOwnerService;

        public DetailsModel(IProjectOwnerService projectOwnerService)
        {
            _projectOwnerService = projectOwnerService;
        }

        public Models.ProjectOwner ProjectOwner { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProjectOwner = await _projectOwnerService.GetProjectOwnerByIdAsync(id.Value);

            if (ProjectOwner == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}