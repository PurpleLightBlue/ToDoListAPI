using System.ComponentModel.DataAnnotations;

namespace ToDoList.Domain.Models
{
    public class ToDoItem
    {
        [Key]
        public int Id { get; set; }  // Primary key
        public string Text { get; set; } = string.Empty;  // To-do text
        public bool IsCompleted { get; set; } = false;  // Check if it's completed
    }

}
