namespace DietBowl.Models
{
    public class DietRecipe
    {
        public int DietId { get; set; }
        public Diet Diet { get; set; }
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
        public bool IsConsumed { get; set; } = false;
    }
}
