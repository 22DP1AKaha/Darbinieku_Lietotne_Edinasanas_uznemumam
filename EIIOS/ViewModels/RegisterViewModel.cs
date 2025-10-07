using System.ComponentModel.DataAnnotations;

namespace EIIOS.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                  ErrorMessageResourceName = "FirstNameRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                           ErrorMessageResourceName = "FirstNameLength")]
        [Display(Name = "FirstName", ResourceType = typeof(Resources.ViewModels.AccountViewModel))]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                  ErrorMessageResourceName = "LastNameRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                           ErrorMessageResourceName = "LastNameLength")]
        [Display(Name = "LastName", ResourceType = typeof(Resources.ViewModels.AccountViewModel))]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                  ErrorMessageResourceName = "UsernameRequired")]
        [StringLength(50, MinimumLength = 3,
                      ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                      ErrorMessageResourceName = "UsernameLength")]
        [Display(Name = "Username", ResourceType = typeof(Resources.ViewModels.AccountViewModel))]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                  ErrorMessageResourceName = "EmailRequired")]
        [StringLength(100, ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                           ErrorMessageResourceName = "EmailLength")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                      ErrorMessageResourceName = "EmailInvalid")]
        [Display(Name = "Email", ResourceType = typeof(Resources.ViewModels.AccountViewModel))]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                  ErrorMessageResourceName = "PasswordRequired")]
        [StringLength(100, MinimumLength = 6,
                      ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                      ErrorMessageResourceName = "PasswordLength")]
        [Display(Name = "Password", ResourceType = typeof(Resources.ViewModels.AccountViewModel))]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                  ErrorMessageResourceName = "ConfirmPasswordRequired")]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(Resources.ViewModels.AccountViewModel))]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                             ErrorMessageResourceName = "PasswordsDoNotMatch")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "AcceptTerms", ResourceType = typeof(Resources.ViewModels.AccountViewModel))]
        [Range(typeof(bool), "true", "true",
               ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
               ErrorMessageResourceName = "AcceptTermsRequired")]
        public bool AcceptTerms { get; set; }
    }
}