using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models;
using Offers.Services.Company;

namespace Offers.Pages.Companies
{
    [Authorize(Policy = "CanDeleteCompany")]
    public class DeleteModel : PageModel
    {
        private readonly ICompanyService _companyService;

        public DeleteModel(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [BindProperty]
        public Company? Company { get; set; } // Marked as nullable to align with potential null assignment

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Company = await _companyService.GetCompanyByIdAsync(id.Value);

            if (Company == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Company = await _companyService.GetCompanyByIdAsync(id.Value);

            if (Company != null)
            {
                await _companyService.DeleteCompanyAsync(id.Value);
            }

            return RedirectToPage("./Index");
        }
    }
}