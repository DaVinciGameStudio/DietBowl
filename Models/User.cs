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
        public string PhoneNumber {get; set;}

        // Relacja 1:1 z preference
        public Preference Preference { get; set; }

        // Relacja jeden do wielu z bodyParameter
        public List<BodyParameter> BodyParameters {get; set;} = new List<BodyParameter>();

        // Relacja jeden do wielu z diets
        public List<Diet> Diets { get; set; } = new List<Diet>();
    }
}