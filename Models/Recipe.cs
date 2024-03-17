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
        public String Title {get; set;}
        public String Ingedients {get; set;}
        public String Instructions {get; set;}
        public double Protein {get; set;}
        public double Fat {get; set;}
        public double Carbohydrate {get; set;}
        public double Calories {get; set;}
    }
}