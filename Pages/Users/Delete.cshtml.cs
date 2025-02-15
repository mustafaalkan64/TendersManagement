using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    public DeleteModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty]
    public IdentityUser User { get; set; }
    public List<string> UserRoles { get; set; } = new List<string>();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        User = await _userManager.FindByIdAsync(id);

        if (User == null)
        {
            return NotFound();
        }

        UserRoles = (await _userManager.GetRolesAsync(User)).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        User = await _userManager.FindByIdAsync(id);

        if (User != null)
        {
            // Prevent deleting the admin user
            if (await _userManager.IsInRoleAsync(User, "Admin"))
            {
                ModelState.AddModelError("", "Cannot delete admin user.");
                UserRoles = (await _userManager.GetRolesAsync(User)).ToList();
                return Page();
            }

            var result = await _userManager.DeleteAsync(User);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                UserRoles = (await _userManager.GetRolesAsync(User)).ToList();
                return Page();
            }
        }

        return RedirectToPage("./Index");
    }
} 