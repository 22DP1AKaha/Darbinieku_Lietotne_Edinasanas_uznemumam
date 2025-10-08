using System.ComponentModel.DataAnnotations;

namespace EIIOS.ViewModels
{
    public class EmployeeViewModel  
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.EmployeeViewModel),
                  ErrorMessageResourceName = "UsernameRequired")]
        [StringLength(50, MinimumLength = 3, ErrorMessageResourceType = typeof(Resources.ViewModels.EmployeeViewModel),
                      ErrorMessageResourceName = "UsernameLength")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.EmployeeViewModel),
                  ErrorMessageResourceName = "EmailRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources.ViewModels.EmployeeViewModel),
                      ErrorMessageResourceName = "EmailInvalid")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.EmployeeViewModel),
                  ErrorMessageResourceName = "FirstNameRequired")]
        [StringLength(50, MinimumLength = 2, ErrorMessageResourceType = typeof(Resources.ViewModels.EmployeeViewModel),
                      ErrorMessageResourceName = "FirstNameLength")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.EmployeeViewModel),
                  ErrorMessageResourceName = "LastNameRequired")]
        [StringLength(50, MinimumLength = 2, ErrorMessageResourceType = typeof(Resources.ViewModels.EmployeeViewModel),
                      ErrorMessageResourceName = "LastNameLength")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 6, ErrorMessageResourceType = typeof(Resources.ViewModels.EmployeeViewModel),
                      ErrorMessageResourceName = "PasswordLength")]
        public string? Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.ViewModels.EmployeeViewModel),
                  ErrorMessageResourceName = "RoleRequired")]
        public string Role { get; set; } = "Employee";

        public bool IsActive { get; set; } = true;

        public bool IsEmailVerified { get; set; }

        public DateTime CreatedAt { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }

    public class EmployeeListViewModel
    {
        public List<EmployeeViewModel> Employees { get; set; } = new();
        public string SearchTerm { get; set; } = string.Empty;
        public string RoleFilter { get; set; } = string.Empty;
        public string StatusFilter { get; set; } = string.Empty;
    }
}