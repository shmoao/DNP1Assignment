using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using Entities;
using ApiContracts;
using System.Linq;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostRepository _postRepo;

        public PostsController(IPostRepository postRepo)
        {
            _postRepo = postRepo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PostDto>> GetAll()
        {
            var items = _postRepo.GetManyAsync()
                .Select(p => new PostDto { Id = p.Id, Title = p.Title, Body = p.Body, UserId = p.UserId })
                .ToList();
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PostDto>> GetById(int id)
        {
            var p = await _postRepo.GetSingleAsync(id);
            if (p == null) return NotFound();
            return Ok(new PostDto { Id = p.Id, Title = p.Title, Body = p.Body, UserId = p.UserId });
        }

        [HttpPost]
        public async Task<ActionResult<PostDto>> Create([FromBody] CreatePostDto dto)
        {
            var created = await _postRepo.AddAsync(new Post { Title = dto.Title, Body = dto.Body, UserId = dto.UserId });
            var result = new PostDto { Id = created.Id, Title = created.Title, Body = created.Body, UserId = created.UserId };
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreatePostDto dto)
        {
            var existing = await _postRepo.GetSingleAsync(id);
            if (existing == null) return NotFound();
            existing.Title = dto.Title;
            existing.Body = dto.Body;
            existing.UserId = dto.UserId;
            await _postRepo.UpdateAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _postRepo.GetSingleAsync(id);
            if (existing == null) return NotFound();
            await _postRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}
