namespace ToDoList.Application.DTOs
{
    public class ToDoItemCreateDTO
    {
        public string Text { get; set; }
        public bool IsCompleted { get; set; } = false;
    }
}
