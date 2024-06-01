using System.ComponentModel.DataAnnotations;

namespace DietBowl.ViewModel
{
    public class NutritionalRequirementsVM
    {
        public int UserId { get; set; }

        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbohydrate { get; set; }
    }
}
