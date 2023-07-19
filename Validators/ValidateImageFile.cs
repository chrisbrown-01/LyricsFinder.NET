using System.ComponentModel.DataAnnotations;

namespace LyricsFinder.NET.Validators
{
    /// <summary>
    /// Validate that image url link is to a jpg, jpeg, or png image
    /// </summary>
    public class ValidateImageFile : ValidationAttribute
    {
        // TODO: rewrite with proper cybersecurity checks
        private const string PNG = "png";
        private const string JPG = "jpg";
        private const string JPEG = "jpeg";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string imageUrl = value.ToString();

                if (imageUrl.Length < 4) return new ValidationResult("Image link is not for a png/jpg/jpeg image.");

                string imageUrlSuffixShort = imageUrl.Substring(imageUrl.Length - 3);
                string imageUrlSuffixLong = imageUrl.Substring(imageUrl.Length - 4);

                if (imageUrlSuffixShort.Equals(PNG) || imageUrlSuffixShort.Equals(JPG))
                {
                    return ValidationResult.Success;
                }
                else if (imageUrlSuffixLong.Equals(JPEG))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Image link is not for a png/jpg/jpeg image.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
