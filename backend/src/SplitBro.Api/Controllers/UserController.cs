using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using SplitBro.Application.Service;
using static SplitBro.Application.Dto.Dto;

namespace SplitBro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController :ControllerBase
    {
        private readonly UserManagerService _userManagerService;
        public UserController(UserManagerService userManagerService) 
        {
            _userManagerService = userManagerService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateUser(CreateUserRequest request)
        {
            var user = await _userManagerService.CreateUserAsync(request);

            return Ok(user);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateUser(int id, UpdateUserRequest request)
        {
            var user = await _userManagerService.UpdateUserAsync(id, request);

            return Ok(user);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetUserById(int id)
        {
            var user = await _userManagerService.GetUserByIdAsync(id);
            return Ok(user);
        }
        [HttpGet("email")]
        public async Task<ActionResult> GetUserByEmail([FromQuery]string email)
        {
            var user = await _userManagerService.GetUserByEmailAsync(email);
            return Ok(user);
        }

    }
}
