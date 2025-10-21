using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;
using Entities;
using ApiContracts;
using System.Linq;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet]
        public ActionResult<IEnumerable<UserDto>> GetAll()
        {
            var users = _userRepo.GetManyAsync()
                .Select(u => new UserDto { Id = u.Id, UserName = u.UserName })
                .ToList();
            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _userRepo.GetSingleAsync(id);
            if (user == null) return NotFound();
            return Ok(new UserDto { Id = user.Id, UserName = user.UserName });
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto)
        {
            var created = await _userRepo.AddAsync(new User
            {
                UserName = dto.UserName,
                Password = dto.Password
            });

            var result = new UserDto { Id = created.Id, UserName = created.UserName };
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUserDto dto)
        {
            var existing = await _userRepo.GetSingleAsync(id);
            if (existing == null) return NotFound();
            existing.UserName = dto.UserName;
            existing.Password = dto.Password;
            await _userRepo.UpdateAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _userRepo.GetSingleAsync(id);
            if (existing == null) return NotFound();
            await _userRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}
