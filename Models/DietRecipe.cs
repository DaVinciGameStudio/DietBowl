using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DietBowl.Models
{
    public class DietRecipe
    {
        public int Id { get; set; }
        public int DietId {get; set;}
        public virtual Diet Diet {get; set;}
        public int RecipeId {get; set;}
        public virtual Recipe Recipe {get; set;}
    }
}