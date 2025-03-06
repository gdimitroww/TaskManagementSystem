using Microsoft.AspNetCore.Identity;

namespace TaskManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Additional properties can be added here
        
        // Navigation property for tasks
        public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
} 