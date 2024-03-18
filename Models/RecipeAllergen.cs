using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DietBowl.Models
{
    public class RecipeAllergen
    {
        public int Id { get; set; }
        public int RecipeId {get; set;}
        public virtual Recipe Recipe {get; set;}
        public int AllergenId {get; set;}
        public virtual Allergen Allergen {get; set;}
    }
}