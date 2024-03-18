using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DietBowl.Models
{
    public class PreferenceAllergen
    {
        public int Id { get; set; }
        public int PreferenceId {get; set;}
        public virtual Preference Preference {get; set;}

        public int AllergenId {get; set;}
        public virtual Allergen Allergen {get; set;}
    }
}