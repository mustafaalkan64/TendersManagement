using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Models
{
    public class Equipment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        // Navigation property
        public ICollection<EquipmentModel> Models { get; set; }
    }
}
