using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace Pages.ProjectOwner
{
    [Authorize(Policy = "CanEditOwner")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private IMemoryCache _cache;

        public EditModel(ApplicationDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [BindProperty]
        public Models.ProjectOwner ProjectOwner { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProjectOwner = await _context.ProjectOwners.FindAsync(id);

            if (ProjectOwner == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(ProjectOwner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                StatusMessage = "Proje sahibi başarıyla güncellendi.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectOwnerExists(ProjectOwner.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProjectOwnerExists(int id)
        {
            return _context.ProjectOwners.Any(e => e.Id == id);
        }
    }
} 