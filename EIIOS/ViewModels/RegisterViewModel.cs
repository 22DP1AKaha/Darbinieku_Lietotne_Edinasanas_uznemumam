using System.ComponentModel.DataAnnotations;

namespace EIIOS.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "FirstNameRequired")]
        [StringLength(50, ErrorMessage = "FirstNameLength")]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "LastNameRequired")]
        [StringLength(50, ErrorMessage = "LastNameLength")]
        [Display(Name = "LastName")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "UsernameRequired")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "UsernameLength")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "EmailRequired")]
        [StringLength(100, ErrorMessage = "EmailLength")]
        [EmailAddress(ErrorMessage = "EmailInvalid")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "PasswordRequired")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "PasswordLength")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "ConfirmPasswordRequired")]
        [Display(Name = "ConfirmPassword")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "PasswordsDoNotMatch")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "AcceptTerms")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "AcceptTermsRequired")]
        public bool AcceptTerms { get; set; }
    }
}
