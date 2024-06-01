using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DietBowl.Models
{
    public class Diet
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbohydrate { get; set; }
        public double Calories { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public List<DietRecipe> DietRecipes { get; set; } = new List<DietRecipe>();
    }
}