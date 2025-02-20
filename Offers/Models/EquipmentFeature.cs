using Models;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class EquipmentFeature
    {
        public int Id { get; set; }

        [Required]
        public int EquipmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string FeatureKey { get; set; }

        [Required]
        [StringLength(2000)]
        public string FeatureValue { get; set; }

        public Equipment Equipment { get; set; }
    }
}
