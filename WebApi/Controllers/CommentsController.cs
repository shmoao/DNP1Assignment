using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using Entities;
using ApiContracts;
using System.Linq;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;

        public CommentsController(ICommentRepository commentRepo)
        {
            _commentRepo = commentRepo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommentDto>> GetAll()
        {
            var items = _commentRepo.GetManyAsync()
                .Select(c => new CommentDto { Id = c.Id, Body = c.Body, UserId = c.UserId, PostId = c.PostId })
                .ToList();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CommentDto>> GetById(int id)
        {
            var c = await _commentRepo.GetSingleAsync(id);
            if (c == null) return NotFound();
            return Ok(new CommentDto { Id = c.Id, Body = c.Body, UserId = c.UserId, PostId = c.PostId });
        }

        [HttpPost]
        public async Task<ActionResult<CommentDto>> Create([FromBody] CreateCommentDto dto)
        {
            var created = await _commentRepo.AddAsync(new Comment { Body = dto.Body, UserId = dto.UserId, PostId = dto.PostId });
            var result = new CommentDto { Id = created.Id, Body = created.Body, UserId = created.UserId, PostId = created.PostId };
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateCommentDto dto)
        {
            var existing = await _commentRepo.GetSingleAsync(id);
            if (existing == null) return NotFound();
            existing.Body = dto.Body;
            existing.UserId = dto.UserId;
            existing.PostId = dto.PostId;
            await _commentRepo.UpdateAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _commentRepo.GetSingleAsync(id);
            if (existing == null) return NotFound();
            await _commentRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}
