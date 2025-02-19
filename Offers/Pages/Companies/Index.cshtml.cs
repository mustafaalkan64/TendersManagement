using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models;
namespace Offers.Pages.Companies
{
    [Authorize(Roles = "Admin, User")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Company> Companies { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Companies = await _context.Companies.OrderByDescending(x => x.CreatedDate).ToListAsync();
        }
    }
} 