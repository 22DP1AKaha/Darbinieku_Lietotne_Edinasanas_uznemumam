using System.ComponentModel.DataAnnotations;

namespace EIIOS.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                  ErrorMessageResourceName = "UsernameRequired")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                  ErrorMessageResourceName = "PasswordRequired")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
