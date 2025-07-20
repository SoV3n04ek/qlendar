using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QlendarBackend.Qlendar.Models;
using QlendarBackend.Qlendar.Services;
using System.Security.Claims;

namespace QlendarBackend.Qlendar.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly TodoService _todoService;

        public TodoController(TodoService todoService)
        {
            _todoService = todoService;
        }

        // GET: api/Todo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var todos = await _todoService.GetTodosForUserAsync(userId);
            return Ok(todos);
        }

        // GET: api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Todo>> GetTodo(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var todo = await _todoService.GetTodoByIdAsync(id, userId);
                return Ok(todo);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/Todo
        [HttpPost]
        public async Task<ActionResult<Todo>> CreateTodo([FromBody] Todo todo)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var createdTodo = await _todoService.CreateTodoAsync(todo, userId);
            return CreatedAtAction(nameof(GetTodo), new { id = createdTodo.Id }, createdTodo);
        }

        // PUT: api/Todo/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Todo>> UpdateTodo(int id, [FromBody] Todo todo)
        {
            if (id != todo.Id)
                return BadRequest(new { message = "Id mismatch" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var updatedTodo = await _todoService.UpdateTodoAsync(todo, userId);
                return Ok(updatedTodo);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await _todoService.DeleteTodoAsync(id, userId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}