using System.ComponentModel.DataAnnotations;

namespace EIIOS.ViewModels
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                  ErrorMessageResourceName = "UsernameRequired")]
        [StringLength(50, MinimumLength = 3,
                      ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                      ErrorMessageResourceName = "UsernameLength")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$",
                          ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                          ErrorMessageResourceName = "UsernameInvalid")]
        public string Username { get; set; } = string.Empty;

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

        // Display only - cannot be edited
        public string Email { get; set; } = string.Empty;
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                  ErrorMessageResourceName = "CurrentPasswordRequired")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                  ErrorMessageResourceName = "NewPasswordRequired")]
        [StringLength(100, MinimumLength = 6,
                      ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                      ErrorMessageResourceName = "PasswordLength")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                  ErrorMessageResourceName = "ConfirmPasswordRequired")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                                ErrorMessageResourceName = "PasswordsDoNotMatch")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}