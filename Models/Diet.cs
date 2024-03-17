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
        public int Id_User {get; set;}
    }
}