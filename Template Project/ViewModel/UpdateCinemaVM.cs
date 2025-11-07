using System.ComponentModel.DataAnnotations;
using Template_Project.Validations;

namespace Template_Project.ViewModel
{
    public class UpdateCinemaVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "هذا الحقل مطلوب")]
        [MinLength(3)]
        [MaxLength(20)]
        public string Name { get; set; }
        public string Image { get; set; } = "defaultImg.png";

        [AllowedExtention(new[] { ".png",".jpg",".jpeg",".gif"})]
        public IFormFile? FormImg { get; set; }
    }
}
