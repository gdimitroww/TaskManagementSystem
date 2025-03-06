using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.ViewModels
{
    public class ResendEmailConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
} 