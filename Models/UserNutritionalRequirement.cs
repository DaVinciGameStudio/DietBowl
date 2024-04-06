namespace DietBowl.Models
{
    public class UserNutritionalRequirement
    {
        public int Id { get; set; }
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbohydrate { get; set; }

        // Relacja 1:1 z User
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
