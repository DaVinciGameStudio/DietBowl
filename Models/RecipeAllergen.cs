using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DietBowl.Models
{
    public class RecipeAllergen
    {
        public int Id {get; set;}
        public int Id_Recipe {get; set;}
        public int Id_Allergen {get; set;}
    }
}