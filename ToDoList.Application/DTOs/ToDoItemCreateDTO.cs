namespace ToDoList.Application.DTOs
{

    /// <summary>
    /// Represents the data transfer object for creating a to-do item.
    /// </summary>
    public class ToDoItemCreateDTO
    {
        /// <summary>
        /// Gets or sets the text of the to-do item.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the to-do item is completed.
        /// </summary>
        public bool IsCompleted { get; set; } = false;
    }
}
