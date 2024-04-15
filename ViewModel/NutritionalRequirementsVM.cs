using System.ComponentModel.DataAnnotations;

namespace DietBowl.ViewModel
{
    public class NutritionalRequirementsVM
    {
        public int UserId { get; set; }

        [Required]
        [Range(1000, 5000)]
        public double Calories { get; set; }

        [Required]
        [Range(10, 300)]
        public double Protein { get; set; }

        [Required]
        [Range(10, 300)]
        public double Fat { get; set; }

        [Required]
        [Range(10, 500)]
        public double Carbohydrate { get; set; }
    }
}
