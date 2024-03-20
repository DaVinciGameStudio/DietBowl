using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DietBowl.Models
{
    public class Allergen
    {
        public int Id {get; set;}
        public string Name {get; set;}

        //relacja wiele do wielu z Recipe
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();

        //relacja wiele do wielu z Preference
        public List<Preference> Preferences { get; set; } = new List<Preference>();
        
    }
}