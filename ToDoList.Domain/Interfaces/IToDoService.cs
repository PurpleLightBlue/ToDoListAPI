using ToDoList.Domain.Models;

namespace ToDoList.Domain.Interfaces
{
    public interface IToDoService
    {
        Task<IEnumerable<ToDoItem>> GetAllAsync();
        Task<ToDoItem?> GetByIdAsync(int id);
        Task<ToDoItem> AddAsync(ToDoItem toDoItem);
        Task UpdateAsync(ToDoItem toDoItem);
        Task DeleteAsync(int id);
    }
}
