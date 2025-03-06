using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementSystem.Models
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
        
        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.ToDo;
        
        [Required]
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        
        // Foreign key for user
        public string? UserId { get; set; }
        
        // Navigation property for user
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
        
        // Created and updated timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
    
    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Done
    }
    
    public enum TaskPriority
    {
        Low,
        Medium,
        High
    }
} 