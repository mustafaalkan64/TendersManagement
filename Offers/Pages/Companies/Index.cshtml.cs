// All common using statements are now in GlobalUsings.cs
namespace Offers.Pages.Companies
{
    [Authorize(Policy = "CanListCompany")]
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