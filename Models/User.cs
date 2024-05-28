using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DietBowl.Models
{
    public class User
    {
        public int Id {get; set;}
        public int? IdDietician {get; set;}
        public int Role {get; set;}

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu email")]
        [RegularExpression(@"^.+@.+\..+$", ErrorMessage = "Adres email musi zawierać znak @")]
        public string Email {get; set;}

        [Required(ErrorMessage = "Pole Hasło jest wymagane.")]
        [StringLength(100, ErrorMessage = "Hasło musi mieć przynajmniej {2} znaków.", MinimumLength = 8)]
        public string Password {get; set;}
        public string FirstName {get; set;}
        public string LastName {get; set;}
        //public int Age {get; set;}

        [CustomValidation(typeof(User), nameof(ValidateDateOfBirth))]
        [Required(ErrorMessage = "Data urodzenia jest wymagana.")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public string Sex {get; set;}
        public string PhoneNumber {get; set;}

        // Relacja 1:1 z preference
        public Preference Preference { get; set; } = null;

        // Relacja 1:1 z userNutritionalRequirement
        public UserNutritionalRequirement UserNutritionalRequirement { get; set; } = null;

        // Relacja jeden do wielu z bodyParameter
        public List<BodyParameter> BodyParameters {get; set;} = new List<BodyParameter>();

        // Relacja jeden do wielu z diets
        public List<Diet> Diets { get; set; } = new List<Diet>();

        //public static implicit operator User(string v)
        //{
        //    throw new NotImplementedException();
        //}

        public static ValidationResult ValidateDateOfBirth(DateTime dateOfBirth, ValidationContext context)
        {
            if (dateOfBirth > DateTime.Now)
            {
                return new ValidationResult("Data urodzenia nie może być w przyszłości.");
            }
            return ValidationResult.Success;
        }
    }
}