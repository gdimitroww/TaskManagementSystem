using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class TaskItemViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Due date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(1);
        
        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public TaskStatus Status { get; set; } = TaskStatus.ToDo;
        
        [Required(ErrorMessage = "Priority is required")]
        [Display(Name = "Priority")]
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        
        public string? UserId { get; set; }
    }
    
    public class TaskListViewModel
    {
        public IEnumerable<TaskItem> Tasks { get; set; } = new List<TaskItem>();
        public string StatusFilter { get; set; } = "All";
        public string SortOrder { get; set; } = "dueDate_asc";
    }
} 