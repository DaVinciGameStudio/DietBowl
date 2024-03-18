using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DietBowl.Models
{
    public class DietRecipe
    {
        public int DietId {get; set;}
        public Diet Diet {get; set;}
        public int RecipeId {get; set;}
        public Recipe Recipe {get; set;}
    }
}