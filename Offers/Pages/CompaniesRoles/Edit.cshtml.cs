using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Pages.CompaniesRoles
{
    [Authorize(Policy = "CanEditCompaniesRoles")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public EditModel(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        [BindProperty]
        public Models.CompaniesRoles CompaniesRoles { get; set; }

        public SelectList CompaniesRequests { get; set; }
        public SelectList RolesRequests { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CompaniesRoles = await _context.CompaniesRoles
                .Include(c => c.Company)
                .Include(c => c.Role).FirstOrDefaultAsync(m => m.Id == id);

            if (CompaniesRoles == null)
            {
                return NotFound();
            }

            CompaniesRequests = new SelectList(await _context.Companies.OrderBy(c => c.Name).ToListAsync(), "Id", "Name");
            RolesRequests = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                 CompaniesRequests = new SelectList(await _context.Companies.OrderBy(c => c.Name).ToListAsync(), "Id", "Name");
                 RolesRequests = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Id", "Name");
                return Page();
            }

            // Check if exists to avoid duplicates (excluding current one)
            bool exists = await _context.CompaniesRoles.AnyAsync(cr => cr.CompanyId == CompaniesRoles.CompanyId && cr.RoleId == CompaniesRoles.RoleId && cr.Id != CompaniesRoles.Id);
            if (exists)
            {
                ModelState.AddModelError(string.Empty, "This company already has this role.");
                 CompaniesRequests = new SelectList(await _context.Companies.OrderBy(c => c.Name).ToListAsync(), "Id", "Name");
                 RolesRequests = new SelectList(await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync(), "Id", "Name");
                return Page();
            }

            _context.Attach(CompaniesRoles).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompaniesRolesExists(CompaniesRoles.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            TempData["SuccessMessage"] = "İşlem başarıyla tamamlandı.";
            return RedirectToPage("./Index");
        }

        private bool CompaniesRolesExists(int id)
        {
            return _context.CompaniesRoles.Any(e => e.Id == id);
        }
    }
}
