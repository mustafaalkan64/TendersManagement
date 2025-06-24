﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Offers.Permissions
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public PermissionHandler(UserManager<IdentityUser> userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var user = await _userManager.GetUserAsync(context.User);
            if (user == null) return;

            var userRoles = await _userManager.GetRolesAsync(user);

            var rp = await _dbContext.RolePermissions
                .Include(c => c.Role)
                .Include(c => c.Permission).ToListAsync();


            var rolePermissions = rp
                .Where(rp => userRoles.Contains(rp.Role.Name))
                .Select(rp => rp.Permission.Name)
                .ToList();

            if (rolePermissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }
        }
    }
}
