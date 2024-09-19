using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.Interfaces;
using ToDoList.Domain.Models;

namespace ToDoList.Infrastructure.Repositories
{
    public class ToDoRepository : IToDoRepository
    {
        private readonly AppDbContext _context;

        public ToDoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ToDoItem> AddAsync(ToDoItem toDoItem)
        {
            _context.ToDoItems.Add(toDoItem);
            await _context.SaveChangesAsync();
            return toDoItem;
        }

        public async Task DeleteAsync(int id)
        {
            var toDoItem = await _context.ToDoItems.FindAsync(id);
            if (toDoItem != null)
            {
                _context.ToDoItems.Remove(toDoItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ToDoItem>> GetAllAsync()
        {
            return await _context.ToDoItems.ToListAsync();
        }

        public async Task<ToDoItem?> GetByIdAsync(int id)
        {
            return await _context.ToDoItems.FindAsync(id);
        }

        public async Task UpdateAsync(ToDoItem toDoItem)
        {
            _context.ToDoItems.Update(toDoItem);
            await _context.SaveChangesAsync();
        }
    }
}
