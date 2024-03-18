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
        public string Ingedients {get; set;}
        public string Instructions {get; set;}
        public double Protein {get; set;}
        public double Fat {get; set;}
        public double Carbohydrate {get; set;}
        public double Calories {get; set;}
        
        //relacja z Diet
        public int DietId { get; set; }
        public virtual Diet Diet { get; set; }

        //realacja z Allergen
        //public int AllergenId {get; set;}
        public virtual ICollection<Allergen> Allergens { get; set; }
    }
}