using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Models;
using Microsoft.AspNetCore.Authorization;

namespace Pages.ProjectOwner
{
    [Authorize(Policy = "CanAddOwner")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.ProjectOwner ProjectOwner { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.ProjectOwners.Add(ProjectOwner);
            await _context.SaveChangesAsync();

            StatusMessage = "Proje sahibi başarıyla oluşturuldu.";
            return RedirectToPage("./Index");
        }
    }
} 