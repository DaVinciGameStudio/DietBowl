using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DietBowl.Models
{
    public class User
    {
        public int Id {get; set;}
        public int? IdDietician {get; set;}
        public string Role {get; set;}
        public string Email {get; set;}
        public string Password {get; set;}
        public string FirstName {get; set;}
        public string LastName {get; set;}
        public int Age {get; set;}
        public string Sex {get; set;}

        // Relacja 1:1 z Preference
        public virtual Preference Preference { get; set; }
        // Relacja 1:1 z BodyParameter
        public virtual BodyParameter BodyParameter {get; set;}

        public virtual List<Diet> Diets { get; set; }
    }
}