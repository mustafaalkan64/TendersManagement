using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public EditModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [BindProperty]
    public IdentityUser UserEdit { get; set; }

    public List<string> UserRoles { get; set; } = new List<string>();
    public List<string> AvailableRoles { get; set; } = new List<string>();

    [BindProperty]
    public List<string> SelectedRoles { get; set; } = new List<string>();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        UserEdit = await _userManager.FindByIdAsync(id);

        if (UserEdit == null)
        {
            return NotFound();
        }

        UserRoles = (await _userManager.GetRolesAsync(UserEdit)).ToList();
        AvailableRoles = _roleManager.Roles.Select(x => x.Name).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByIdAsync(UserEdit.Id);
        if (user == null)
        {
            return NotFound();
        }

        user.Email = UserEdit.Email;
        user.UserName = UserEdit.Email;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return Page();
        }

        // Update roles
        var currentRoles = await _userManager.GetRolesAsync(user);
        SelectedRoles ??= new List<string>();

        var rolesToRemove = currentRoles.Except(SelectedRoles);
        var rolesToAdd = SelectedRoles.Except(currentRoles);

        await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
        await _userManager.AddToRolesAsync(user, rolesToAdd);

        return RedirectToPage("./Index");
    }
} 