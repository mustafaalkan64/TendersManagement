using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Pages.ProjectOwner
{
    [Authorize(Policy = "CanListOwner")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public IList<Models.ProjectOwner> ProjectOwners { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.ProjectOwners.AsQueryable();
            if (!string.IsNullOrEmpty(SearchString))
            {
                var searchTerm = SearchString.ToLower();
                query = query.Where(em => em.Name.Contains(SearchString, StringComparison.OrdinalIgnoreCase));
            }
            ProjectOwners = await query
                .OrderBy(p => p.Name)
                .ToListAsync();
        }
    }
}