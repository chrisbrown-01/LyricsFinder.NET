using System.ComponentModel.DataAnnotations;

namespace LyricsFinder.NET.Validators
{
    /// <summary>
    /// Validate that image url link is to a jpg, jpeg, or png image
    /// </summary>
    public class ValidateImageFile : ValidationAttribute
    {
        private const string PNG = "png";
        private const string JPG = "jpg";
        private const string JPEG = "jpeg";

#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0057:Use range operator", Justification = "<Pending>")]
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
#pragma warning restore CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
        {
            if (value != null)
            {
                string imageUrl = value.ToString()!;

                if (imageUrl.Length < 4) return new ValidationResult("Image link is not for a png/jpg/jpeg image.");

                string imageUrlSuffixShort = imageUrl.Substring(imageUrl.Length - 3);
                string imageUrlSuffixLong = imageUrl.Substring(imageUrl.Length - 4);

                if (imageUrlSuffixShort.Equals(PNG) || imageUrlSuffixShort.Equals(JPG))
                {
                    return ValidationResult.Success!;
                }
                else if (imageUrlSuffixLong.Equals(JPEG))
                {
                    return ValidationResult.Success!;
                }
                else
                {
                    return new ValidationResult("Image link is not for a png/jpg/jpeg image.");
                }
            }

            return ValidationResult.Success!;
        }
    }
}
