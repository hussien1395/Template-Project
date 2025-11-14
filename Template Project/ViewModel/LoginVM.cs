using System.ComponentModel.DataAnnotations;

namespace Template_Project.ViewModel
{
    public class LoginVM
    {
        public int Id { get; set; }
        public string UserNameOrEmail { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
