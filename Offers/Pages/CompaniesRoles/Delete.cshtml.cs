using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Threading.Tasks;

namespace Offers.Pages.CompaniesRoles
{
    [Authorize(Policy = "CanDeleteCompaniesRoles")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.CompaniesRoles CompaniesRoles { get; set; }

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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CompaniesRoles = await _context.CompaniesRoles.FindAsync(id);

            if (CompaniesRoles != null)
            {
                _context.CompaniesRoles.Remove(CompaniesRoles);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
