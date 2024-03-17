using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DietBowl.Models
{
    public class BodyParameter
    {
        public int Id {get; set;}
        public double Height {get; set;}
        public double Weight {get; set;}
        public double BMI {get; set;}
        public DateTime Date {get; set;}
        
        // Relacja 1:1 z User
        public int UserId {get; set;}
        public virtual User User {get; set;}
    }
}