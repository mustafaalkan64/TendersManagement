using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Permission
    {
        public int Id { get; set; }
        
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string DisplayName { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
