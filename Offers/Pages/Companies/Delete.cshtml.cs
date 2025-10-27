using System.Security.Claims;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteModel(ICompanyService companyService, IHttpContextAccessor httpContextAccessor)
        {
            _companyService = companyService;
            _httpContextAccessor = httpContextAccessor;
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

            var user = _httpContextAccessor.HttpContext?.User;

            var roles = user?.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value.ToLower().Trim()) // rolleri lowercase yapýyoruz
                .ToList() ?? new List<string>();

            if (!user.IsInRole("Admin") && !roles.Contains(Company.Name.ToLower()))
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
                var user = _httpContextAccessor.HttpContext?.User;

                var roles = user?.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value.ToLower().Trim()) // rolleri lowercase yapýyoruz
                    .ToList() ?? new List<string>();

                if (!user.IsInRole("Admin") && !roles.Contains(Company.Name.ToLower()))
                {
                    return NotFound();
                }

                await _companyService.DeleteCompanyAsync(id.Value);
            }

            return RedirectToPage("./Index");
        }
    }
}