using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models
{
    public class EquipmentModel
    {
        public int Id { get; set; }

        [Required]
        public int EquipmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string Model { get; set; }

        [Required]
        [StringLength(100)]
        public string Brand { get; set; }

        [JsonIgnore]
        // Navigation property
        public Equipment Equipment { get; set; }

        public List<EquipmentModelFeature> Features { get; set; } = new List<EquipmentModelFeature>();
    }
}
