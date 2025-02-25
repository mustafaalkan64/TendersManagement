using System.ComponentModel.DataAnnotations;

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
    }
}
