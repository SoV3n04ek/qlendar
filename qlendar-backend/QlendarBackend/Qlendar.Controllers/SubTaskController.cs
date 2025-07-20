using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QlendarBackend.Qlendar.Models;
using QlendarBackend.Qlendar.Services;
using System.Threading.Tasks;

namespace QlendarBackend.Qlendar.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SubTaskController : ControllerBase
    {
        private readonly SubTaskService _subTaskService;

        public SubTaskController(SubTaskService subTaskService)
        {
            _subTaskService = subTaskService;
        }

        // GET: api/SubTask/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubTask>> GetSubTask(int id)
        {
            try
            {
                var subTask = await _subTaskService.GetByIdAsync(id);
                return Ok(subTask);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // POST: api/SubTask
        [HttpPost]
        public async Task<ActionResult<SubTask>> CreateSubTask([FromBody] SubTask subTask)
        {
            var createdSubTask = await _subTaskService.CreateAsync(subTask);
            return CreatedAtAction(nameof(GetSubTask), new { id = createdSubTask.Id }, createdSubTask);
        }

        // PUT: api/SubTask/5
        [HttpPut("{id}")]
        public async Task<ActionResult<SubTask>> UpdateSubTask(int id, [FromBody] SubTask subTask)
        {
            if (id != subTask.Id)
                return BadRequest(new { message = "Id mismatch" });

            try
            {
                var updatedSubTask = await _subTaskService.UpdateAsync(subTask);
                return Ok(updatedSubTask);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE: api/SubTask/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubTask(int id)
        {
            try
            {
                await _subTaskService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}