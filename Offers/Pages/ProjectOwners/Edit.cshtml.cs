using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Offers.Services.ProjectOwner;
using System.Threading.Tasks;

namespace Pages.ProjectOwner
{
    [Authorize(Policy = "CanEditOwner")]
    public class EditModel : PageModel
    {
        private readonly IProjectOwnerService _projectOwnerService;

        public EditModel(IProjectOwnerService projectOwnerService)
        {
            _projectOwnerService = projectOwnerService;
        }

        [BindProperty]
        public Models.ProjectOwner ProjectOwner { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _projectOwnerService.UpdateProjectOwnerAsync(ProjectOwner);
            StatusMessage = "Proje sahibi başarıyla güncellendi.";
            return RedirectToPage("./Index");
        }
    }
}