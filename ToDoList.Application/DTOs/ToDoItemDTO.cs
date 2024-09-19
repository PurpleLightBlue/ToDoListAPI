namespace ToDoList.Application.DTOs;

public class ToDoItemDto
{
    public int Id { get; set; }  // Expose the Id here for display and update
    public string Text { get; set; }
    public bool IsCompleted { get; set; }
}
