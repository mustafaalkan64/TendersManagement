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

        [Range(1, int.MaxValue, ErrorMessage = "Deger 0'dan buyuk olmali")]
        public int? Min { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Deger 0'dan buyuk olmali")]
        public int? Max { get; set; }

        [Required]
        [StringLength(2000)]
        public string FeatureValue { get; set; }

        [Display(Name = "Birim")]
        public int? UnitId { get; set; }

        public Equipment Equipment { get; set; }
        public Unit Unit { get; set; }
    }
}
