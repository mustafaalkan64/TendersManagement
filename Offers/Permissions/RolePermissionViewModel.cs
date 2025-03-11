namespace Offers.Permissions
{
    public class RolePermissionViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<PermissionViewModel> Permissions { get; set; }
    }

    public class PermissionViewModel
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public bool IsAssigned { get; set; }
    }
}
