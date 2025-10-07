using System.ComponentModel.DataAnnotations;

namespace EIIOS.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                  ErrorMessageResourceName = "UsernameRequired")]
        [Display(Name = "Username", ResourceType = typeof(Resources.ViewModels.AccountViewModel))]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.AccountViewModel),
                  ErrorMessageResourceName = "PasswordRequired")]
        [Display(Name = "Password", ResourceType = typeof(Resources.ViewModels.AccountViewModel))]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "RememberMe", ResourceType = typeof(Resources.ViewModels.AccountViewModel))]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}