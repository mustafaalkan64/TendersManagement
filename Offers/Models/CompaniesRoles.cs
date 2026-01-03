using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class CompaniesRoles
    {
        [Key]
        public int Id { get; set; }

        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        public string RoleId { get; set; }
        [ForeignKey("RoleId")]
        public IdentityRole Role { get; set; }
    }
}
