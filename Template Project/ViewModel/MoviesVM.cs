using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing.Drawing2D;

namespace Template_Project.ViewModel
{
    public class MoviesVM
    {
        public Movie Movie { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<SelectListItem> Cinemas { get; set; }
        public IEnumerable<SelectListItem> Actors { get; set; } 
        public List<int> SelectedActors { get; set; } = new();  
    }
}
