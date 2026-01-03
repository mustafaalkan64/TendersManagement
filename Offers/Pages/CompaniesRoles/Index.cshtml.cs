using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Offers.Pages.CompaniesRoles
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.CompaniesRoles> CompaniesRoles { get; set; }

        public async Task OnGetAsync()
        {
            CompaniesRoles = await _context.CompaniesRoles
                .Include(c => c.Company)
                .Include(c => c.Role)
                .ToListAsync();
        }
    }
}
