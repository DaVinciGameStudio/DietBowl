namespace DietBowl.ViewModel
{
    public class AllergenVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> RecipeIds { get; set; } = new List<int>();
        public List<int> PreferenceIds { get; set; } = new List<int>();
    }
}
