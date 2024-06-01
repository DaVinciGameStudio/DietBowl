namespace DietBowl.ViewModel
{
    public class UserMacronutrientsVM
    {
            public int UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
            public string Sex { get; set; }
            public int? WeightGoal { get; set; }
            public string ActivityStatus { get; set; }
            public double Calories { get; set; }
            public double Protein { get; set; }
            public double Fat { get; set; }
            public double Carbohydrate { get; set; }
    }
}
