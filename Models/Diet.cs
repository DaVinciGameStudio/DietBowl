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

        //Reclacja wiele do wielu z Recipe
        public List<Recipe> Recipes { get; set; } = new List<Recipe>();

        //relacja z user jeden do wielu (jeden uzytkownik ma wiele diet ale dana dieta tylko jednego uzytkownika)
        //czyli od tej strony jest to 1:1 (nie znam sie na bazach ale jest dobrze iks de)
        public int UserId {get; set;}
        public User User {get; set;}
    }
}