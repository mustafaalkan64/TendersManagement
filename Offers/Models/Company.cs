using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Company Name is required")]
        [StringLength(150, MinimumLength = 2)]
        [Display(Name = "Company Name")]
        public string Name { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}