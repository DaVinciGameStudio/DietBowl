using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DietBowl.Models
{
    public class Diet
    {
        public int Id {get; set;}
        public DateTime Date {get; set;}
        //public int IdRecipes {get; set;}
        //------
        public virtual ICollection<Recipe> Recipes { get; set; }

        //relacja z User
        public int UserId {get; set;}
        public virtual User User {get; set;}
    }
}