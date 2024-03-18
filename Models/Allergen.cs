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

        //relacja z Recipe
        //public int RecipeId { get; set; } // Klucz obcy wskazujący na przepis
        public virtual List<RecipeAllergen> RecipeAllergens { get; set; } // Powiązanie z przepisem

        //relacja z Preferences
        // public int PreferenceId { get; set; } // Klucz obcy wskazujący na preference
        public virtual List<PreferenceAllergen> PreferenceAllergens { get; set; } // Powiązanie z preference
        
    }
}