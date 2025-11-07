using System.ComponentModel.DataAnnotations;

namespace Template_Project.Validations
{
    public class AllowedExtentionAttribute : ValidationAttribute
    {
        private string[] AllowedExtentions;
        public AllowedExtentionAttribute(string[] allowedExtentions)
        {
            this.AllowedExtentions = allowedExtentions;
        }

        public override bool IsValid(object? value)
        {
         

            if (value is FormFile FormImg)
            {
                
                var ImgExtention = Path.GetExtension(FormImg.FileName);
                if(AllowedExtentions.Contains(ImgExtention))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
