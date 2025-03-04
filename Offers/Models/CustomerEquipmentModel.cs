using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class CompanyEquipmentModel
    {
        public int Id { get; set; }

        [Required]
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        [Required]
        public int EquipmentModelId { get; set; }
        public EquipmentModel EquipmentModel { get; set; }

        [Column(TypeName = "decimal(18,2)")] // Defines precision and scale in DB
        [Range(0.01, 9999999.99, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

    }
}
