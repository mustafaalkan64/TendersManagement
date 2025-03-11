using Microsoft.AspNetCore.Identity;

namespace Models
{
    public class RolePermission
    {
        public int Id {  get; set; }
        public string RoleId { get; set; }
        public IdentityRole Role { get; set; } = null!;

        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;
    }
}
