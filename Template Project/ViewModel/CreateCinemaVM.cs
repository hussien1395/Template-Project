using System.ComponentModel.DataAnnotations;
using Template_Project.Validations;

namespace Template_Project.ViewModel
{
    public class CreateCinemaVM
    {
        [Required(ErrorMessage = "هذا الحقل مطلوب")]
        [MinLength(3)]
        [MaxLength(20)]
        public string Name { get; set; }

        [AllowedExtention(new[] { ".png", ".jpg", ".jpeg", ".gif" })]
        public IFormFile FormImg { get; set; }
    }
}
