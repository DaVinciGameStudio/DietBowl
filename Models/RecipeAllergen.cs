using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DietBowl.Models
{
    public class RecipeAllergen
    {
        public int RecipeId {get; set;}
        public Recipe Recipe {get; set;}
        public int AllergenId {get; set;}
        public Allergen Allergen {get; set;}
    }
}