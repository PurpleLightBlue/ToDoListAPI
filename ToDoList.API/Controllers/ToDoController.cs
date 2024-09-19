using Microsoft.AspNetCore.Mvc;
using ToDoList.Application.DTOs;
using ToDoList.Domain.Interfaces;
using ToDoList.Domain.Models;

namespace ToDoList.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToDoController : ControllerBase
    {
        private readonly IToDoService _toDoService;

        public ToDoController(IToDoService toDoService)
        {
            _toDoService = toDoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
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

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ToDoItemCreateDTO toDoItemCreate)
        {
            var toDoItem = new ToDoItem
            {
                Text = toDoItemCreate.Text,
                IsCompleted = toDoItemCreate.IsCompleted
            };

            var createdTodo = await _toDoService.AddAsync(toDoItem);
            return CreatedAtAction(nameof(GetById), new { id = createdTodo.Id }, createdTodo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ToDoItemDto toDoItemDto)
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _toDoService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("fuzzysearch")]
        public async Task<IActionResult> FuzzySearch([FromQuery] string searchTerm)
        {
            var searchResults = await _toDoService.FuzzySearchAsync(searchTerm);
            return Ok(searchResults);
        }
    }
}
