using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DietBowl.ViewModel
{
    public class PreferenceVM
    {
        //public int Id { get; set; }

        [Required(ErrorMessage = "Opis jest wymagany.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Cel wagowy jest wymagany.")]
        [Range(0, int.MaxValue, ErrorMessage = "Cel wagowy musi być liczbą dodatnią.")]
        public int WeightGoal { get; set; }

        [Required(ErrorMessage = "Status aktywności jest wymagany.")]
        public string ActivityStatus { get; set; }

        // Lista ID wybranych alergenów
        public List<int> SelectedAllergensIds { get; set; } = new List<int>();

        // Lista dostępnych alergenów (do wyświetlenia w formularzu)
        public List<AllergenVM> AvailableAllergens { get; set; } = new List<AllergenVM>();
        public int UserId { get; set; }


        public SelectList ActivityStatusOptions { get; set; }
        public PreferenceVM()
        {
            // Przykładowe wartości, powinny zostać zastąpione dynamicznymi danymi, np. z bazy danych
            ActivityStatusOptions = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Niski", Value = "Low"},
                new SelectListItem { Text = "Średni", Value = "Medium"},
                new SelectListItem { Text = "Wysoki", Value = "High"}
            }, "Value", "Text");
        }
    }
}
