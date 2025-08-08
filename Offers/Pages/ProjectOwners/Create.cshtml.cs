using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Offers.Services.ProjectOwner;
using Models;
using Microsoft.AspNetCore.Authorization;

namespace Pages.ProjectOwner
{
    [Authorize(Policy = "CanAddOwner")]
    public class CreateModel : PageModel
    {
        private readonly IProjectOwnerService _projectOwnerService;

        public CreateModel(IProjectOwnerService projectOwnerService)
        {
            _projectOwnerService = projectOwnerService;
        }

        [BindProperty]
        public Models.ProjectOwner ProjectOwner { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _projectOwnerService.CreateProjectOwnerAsync(ProjectOwner);

            StatusMessage = "Proje sahibi başarıyla oluşturuldu.";
            return RedirectToPage("./Index");
        }
    }
}