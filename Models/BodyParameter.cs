using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DietBowl.Models
{
    public class BodyParameter
    {
        public int Id {get; set;}
        public int IdUser {get; set;}
        public double height {get; set;}
        public double weight {get; set;}
        public double BMI {get; set;}
        public DateTime Date {get; set;}
        
        // Relacja 1:1 z User
        public User User {get; set;}
    }
}