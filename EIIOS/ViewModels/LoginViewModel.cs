using System.ComponentModel.DataAnnotations;

namespace EIIOS.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "UsernameRequired")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "PasswordRequired")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "RememberMe")]
        public bool RememberMe { get; set; }
    }
}
