using System.ComponentModel.DataAnnotations;

namespace Template_Project.Validations
{
    public class CustomLengthAttribute : ValidationAttribute
    {
        private int MxLength;
        private int MnLength;

        public CustomLengthAttribute(int MxLength,int MnLength)
        {
            this.MxLength = MxLength;
            this.MnLength = MnLength;
        }
        public override bool IsValid(object? value)
        {
            if (value is string name)
            {
                return name.Length >= 3 && name.Length <= 100;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"the '{name}' must be > {MnLength} and < {MxLength}";
        }
    }
}
