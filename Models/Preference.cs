using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DietBowl.Models
{
    public class Preference
    {
        public int Id {get; set;}
        public string Description {get; set;}
        public int WeightGoal {get; set;}
        public string ActivityStatus {get; set;}

        //relacja wiele do wielu z Allergen
        public List<Allergen> Allergens { get; set; } = new List<Allergen>();

        // Relacja 1:1 z User
        public int UserId {get; set;}
        public User? User { get; set; }

    }
}