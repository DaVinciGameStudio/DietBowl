using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DietBowl.Models
{
    public class PreferenceAllergen
    {
        public int PreferenceId {get; set;}
        public Preference Preference {get; set;}

        public int AllergenId {get; set;}
        public Allergen Allergen {get; set;}
    }
}