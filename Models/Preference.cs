using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DietBowl.Models
{
    public class Preference
    {
        public int Id {get; set;}
        public int IdUser {get; set;}
        public string Description {get; set;}
        public int IdAllergen {get; set;}
        public int WeightGoal {get; set;}
        public string ActivityStatus {get; set;}

        //relacja z Allergen
        public List<Allergen> Allergens { get; set; }

        // Relacja 1:1 z User
        public User User { get; set; }

    }
}