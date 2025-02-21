using Models;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class EquipmentModelFeature
    {
        public int Id { get; set; }

        [Required]
        public int EquipmentModelId { get; set; }

        [Required]
        [StringLength(100)]
        public string FeatureKey { get; set; }

        [Required]
        [StringLength(500)]
        public string FeatureValue { get; set; }

        [Display(Name = "Birim")]
        public int? UnitId { get; set; }

        public EquipmentModel EquipmentModel { get; set; }

        public Unit Unit { get; set; }
    }
}
