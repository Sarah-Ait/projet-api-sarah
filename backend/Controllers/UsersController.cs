using Microsoft.AspNetCore.Mvc;
using backend.Interfaces;
using backend.Models;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "Utilisateur non trouvé" });
            }

            return Ok(user);
            // ok 
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            var createdUser = await _userService.CreateUserAsync(user);

            if (createdUser == null)
            {
                return BadRequest(new { message = "Données utilisateur invalides" });
            }

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);// renvoie le status 201 Created concretement c est la reponse complete 
        }
    }
}