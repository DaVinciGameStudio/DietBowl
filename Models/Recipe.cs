using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DietBowl.Models
{
    public class Recipe
    {
        public int Id {get; set;}
        public string Title {get; set;}
        public string Category { get; set;}
        public string Ingedients {get; set;}
        public string Instructions {get; set;}
        public double Protein {get; set;}
        public double Fat {get; set;}
        public double Carbohydrate {get; set;}
        public double Calories {get; set;}

        //Relacja wiele do wielu z DietRecipe
        public List<DietRecipe> DietRecipes { get; set; } = new List<DietRecipe>();

        //Relacja wiele do wielu z Allergen
        public List<Allergen> Allergens { get; set; } = new List<Allergen>();

        public double CalculateCalories()
        {
            return (Protein * 4) + (Fat * 9) + (Carbohydrate * 4);
        }
    }
}