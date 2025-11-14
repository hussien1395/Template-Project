namespace Template_Project.ViewModel
{
    public class ResetPasswordVM
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
