using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Models;
using Offers.Permissions;

namespace Pages.Roles
{
    [Authorize(Roles = "Admin")]
    public class EditPermissionsModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public EditPermissionsModel(RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }

        [BindProperty]
        public string RoleId { get; set; }

        [BindProperty]
        public string RoleName { get; set; }

        [BindProperty]
        public List<PermissionViewModel> Permissions { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            RoleId = role.Id;
            RoleName = role.Name;

            Permissions = await _context.Permissions.OrderBy(p => p.DisplayName)
                .Select(p => new PermissionViewModel
                {
                    PermissionId = p.Id,
                    PermissionName = p.DisplayName,
                    IsAssigned = _context.RolePermissions.Any(rp => rp.RoleId == role.Id && rp.PermissionId == p.Id)
                })
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            var role = await _roleManager.FindByIdAsync(RoleId);

            if (role == null)
            {
                return NotFound();
            }

            var existingPermissions = _context.RolePermissions.Where(rp => rp.RoleId == role.Id);
            _context.RolePermissions.RemoveRange(existingPermissions);

            foreach (var permission in Permissions.Where(p => p.IsAssigned))
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permission.PermissionId
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
