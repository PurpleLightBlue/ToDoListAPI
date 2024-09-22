using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.Interfaces;
using ToDoList.Domain.Models;

namespace ToDoList.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for managing to-do items in the database.
    /// </summary>
    public class ToDoRepository : IToDoRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToDoRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public ToDoRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new to-do item to the database.
        /// </summary>
        /// <param name="toDoItem">The to-do item to add.</param>
        /// <returns>The added to-do item.</returns>
        public async Task<ToDoItem> AddAsync(ToDoItem toDoItem)
        {
            _context.ToDoItems.Add(toDoItem);
            await _context.SaveChangesAsync();
            return toDoItem;
        }

        /// <summary>
        /// Deletes a to-do item by ID from the database.
        /// </summary>
        /// <param name="id">The ID of the to-do item to delete.</param>
        public async Task DeleteAsync(int id)
        {
            var toDoItem = await _context.ToDoItems.FindAsync(id);
            if (toDoItem != null)
            {
                _context.ToDoItems.Remove(toDoItem);
                await _context.SaveChangesAsync();
            }
        }


        /// <summary>
        /// Gets all to-do items from the database.
        /// </summary>
        /// <returns>A list of to-do items.</returns>
        public async Task<IEnumerable<ToDoItem>> GetAllAsync()
        {
            return await _context.ToDoItems.ToListAsync();
        }

        /// <summary>
        /// Gets a to-do item by ID from the database.
        /// </summary>
        /// <param name="id">The ID of the to-do item.</param>
        /// <returns>The to-do item, or null if not found.</returns>
        public async Task<ToDoItem?> GetByIdAsync(int id)
        {
            return await _context.ToDoItems.FindAsync(id);
        }

        /// <summary>
        /// Updates an existing to-do item in the database.
        /// </summary>
        /// <param name="toDoItem">The to-do item to update.</param>
        public async Task UpdateAsync(ToDoItem toDoItem)
        {
            _context.ToDoItems.Update(toDoItem);
            await _context.SaveChangesAsync();
        }
    }
}
