namespace ToDoList.Application.DTOs;

/// <summary>
/// Represents a to-do item.
/// </summary>
public class ToDoItemDto
{
    /// <summary>
    /// Gets or sets the ID of the to-do item.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the text of the to-do item.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the to-do item is completed or not.
    /// </summary>
    public bool IsCompleted { get; set; }
}
