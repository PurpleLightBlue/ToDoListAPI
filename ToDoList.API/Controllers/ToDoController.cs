using Microsoft.AspNetCore.Mvc;
using ToDoList.Application.DTOs;
using ToDoList.Domain.Interfaces;
using ToDoList.Domain.Models;

namespace ToDoList.Api.Controllers
{
    /// <summary>
    /// Controller for managing to-do items.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ToDoController : ControllerBase
    {
        private readonly IToDoService _toDoService;
        private readonly ILogger<ToDoController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToDoController"/> class.
        /// </summary>
        /// <param name="toDoService">The to-do service.</param>
        /// <param name="logger">The logger.</param>
        public ToDoController(IToDoService toDoService, ILogger<ToDoController> logger)
        {
            _toDoService = toDoService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all to-do items.
        /// </summary>
        /// <returns>A list of to-do items inside an OkObjectResult</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var toDoItems = await _toDoService.GetAllAsync();

                // Map the domain model to the DTO
                var toDoItemDtos = toDoItems.Select(toDo => new ToDoItemDto
                {
                    Id = toDo.Id,
                    Text = toDo.Text,
                    IsCompleted = toDo.IsCompleted
                });

                return Ok(toDoItemDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all to-do items.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a to-do item by ID.
        /// </summary>
        /// <param name="id">The ID of the to-do item.</param>
        /// <returns>The to-do item inside an OkObjectResult, or NotFound if not found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var toDoItem = await _toDoService.GetByIdAsync(id);
                if (toDoItem == null)
                {
                    return NotFound();
                }

                var toDoItemDto = new ToDoItemDto
                {
                    Id = toDoItem.Id,
                    Text = toDoItem.Text,
                    IsCompleted = toDoItem.IsCompleted
                };

                return Ok(toDoItemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting the to-do item with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Adds a new to-do item.
        /// </summary>
        /// <param name="toDoItemCreate">The to-do item to create.</param>
        /// <returns>The created to-do item inside a CreatedAtAction result.</returns>
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ToDoItemCreateDTO toDoItemCreate)
        {
            try
            {
                var toDoItem = new ToDoItem
                {
                    Text = toDoItemCreate.Text,
                    IsCompleted = toDoItemCreate.IsCompleted
                };

                var createdTodo = await _toDoService.AddAsync(toDoItem);
                return CreatedAtAction(nameof(GetById), new { id = createdTodo.Id }, createdTodo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new to-do item.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Updates an existing to-do item.
        /// </summary>
        /// <param name="id">The ID of the to-do item to update.</param>
        /// <param name="toDoItemDto">The updated to-do item data.</param>
        /// <returns>NoContent if successful, BadRequest if the ID does not match, or InternalServerError if an error occurs.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ToDoItemDto toDoItemDto)
        {
            try
            {
                if (id != toDoItemDto.Id)
                {
                    return BadRequest();
                }

                var toDoItem = new ToDoItem
                {
                    Id = toDoItemDto.Id,
                    Text = toDoItemDto.Text,
                    IsCompleted = toDoItemDto.IsCompleted
                };

                await _toDoService.UpdateAsync(toDoItem);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the to-do item with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a to-do item by ID.
        /// </summary>
        /// <param name="id">The ID of the to-do item to delete.</param>
        /// <returns>NoContent if successful, or InternalServerError if an error occurs.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _toDoService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the to-do item with ID {id}.");
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Performs a fuzzy search for to-do items.
        /// </summary>
        /// <param name="searchTerm">The search term.</param>
        /// <returns>A list of to-do items matching the search term inside an OkObjectResult.</returns>
        [HttpGet("fuzzysearch")]
        public async Task<IActionResult> FuzzySearch([FromQuery] string searchTerm)
        {
            try
            {
                var searchResults = await _toDoService.FuzzySearchAsync(searchTerm);
                return Ok(searchResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while performing a fuzzy search with term '{searchTerm}'.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
