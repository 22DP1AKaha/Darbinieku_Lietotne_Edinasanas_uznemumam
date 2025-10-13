using System.ComponentModel.DataAnnotations;

namespace EIIOS.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                  ErrorMessageResourceName = "FirstNameRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                           ErrorMessageResourceName = "FirstNameLength")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                  ErrorMessageResourceName = "LastNameRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                           ErrorMessageResourceName = "LastNameLength")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                  ErrorMessageResourceName = "UsernameRequired")]
        [StringLength(50, MinimumLength = 3,
                      ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                      ErrorMessageResourceName = "UsernameLength")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                  ErrorMessageResourceName = "EmailRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                           ErrorMessageResourceName = "EmailLength")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                      ErrorMessageResourceName = "EmailInvalid")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                  ErrorMessageResourceName = "PasswordRequired")]
        [StringLength(100, MinimumLength = 6,
                      ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                      ErrorMessageResourceName = "PasswordLength")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                  ErrorMessageResourceName = "ConfirmPasswordRequired")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                             ErrorMessageResourceName = "PasswordsDoNotMatch")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Range(typeof(bool), "true", "true",
               ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
               ErrorMessageResourceName = "AcceptTermsRequired")]
        public bool AcceptTerms { get; set; }
    }
}
