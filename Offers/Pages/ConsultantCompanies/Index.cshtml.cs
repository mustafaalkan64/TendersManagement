using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models;
namespace Offers.Pages.ConsultantCompanies
{
    [Authorize(Policy = "CanListConsultantCompany")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<ConsultantCompany> Companies { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Companies = await _context.ConsultantCompanies.OrderByDescending(x => x.CreatedDate).ToListAsync();
        }
    }
} 