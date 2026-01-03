using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Pages.CompaniesRoles
{

    [Authorize(Policy = "CanAddCompaniesRoles")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateModel(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        [BindProperty]
        public List<int> SelectedCompanyIds { get; set; } = new List<int>();

        [BindProperty]
        public List<string> SelectedRoleIds { get; set; } = new List<string>();

        public SelectList CompaniesRequests { get; set; }
        public SelectList RolesRequests { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            CompaniesRequests = new SelectList(await _context.Companies.OrderBy(c => c.Name).ToListAsync(), "Id", "Name");
            RolesRequests = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!SelectedCompanyIds.Any() || !SelectedRoleIds.Any())
            {
                ModelState.AddModelError(string.Empty, "Please select at least one company and one role.");
                 CompaniesRequests = new SelectList(await _context.Companies.OrderBy(c => c.Name).ToListAsync(), "Id", "Name");
                 RolesRequests = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Id", "Name");
                return Page();
            }

            foreach (var companyId in SelectedCompanyIds)
            {
                foreach (var roleId in SelectedRoleIds)
                {
                    // Check if exists to avoid duplicates
                    bool exists = await _context.CompaniesRoles.AnyAsync(cr => cr.CompanyId == companyId && cr.RoleId == roleId);
                    if (!exists)
                    {
                        _context.CompaniesRoles.Add(new Models.CompaniesRoles
                        {
                            CompanyId = companyId,
                            RoleId = roleId
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "İşlem başarıyla tamamlandı.";
            return RedirectToPage("./Create");
        }
    }
}
