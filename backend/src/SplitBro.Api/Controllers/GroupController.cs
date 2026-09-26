using Microsoft.AspNetCore.Mvc;
using SplitBro.Application.Service;
using static SplitBro.Domain.Enums.GroupDto;

namespace SplitBro.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly GroupManagerService _groupManagerService;

        public GroupController(GroupManagerService groupManagerService)
        {
            _groupManagerService = groupManagerService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
        {
            var group = await _groupManagerService.CreateGroupWithMemberAsync(request);

            return Ok(group);
        }

        [HttpGet("{groupId:int}")]
        public async Task<IActionResult> GetGroup(int groupId)
        {
            var group = await _groupManagerService.GetGroupByIdAsync(groupId);

            return Ok(group);
        }

        [HttpPut("{groupId:int}")]
        public async Task<IActionResult> UpdateGroup(int groupId, [FromBody] UpdateGroupRequest request)
        {
            var group = await _groupManagerService.UpdateGroupAsync(groupId, request);

            return Ok(group);
        }

        [HttpPost("{groupId:int}/members/{userId:int}")]
        public async Task<IActionResult> AddMember(int groupId, int userId)
        {
            var member = await _groupManagerService.AddMemberAsync(groupId,userId);

            return Ok(member);
        }

        [HttpGet("{groupId:int}/members")]
        public async Task<IActionResult> GetMembers(int groupId)
        {
            var members = await _groupManagerService.GetMembersAsync(groupId);

            return Ok(members);
        }

        [HttpDelete("{groupId:int}/members/{userId:int}")]
        public async Task<IActionResult> RemoveMember(int groupId, int userId)
        {
            await _groupManagerService.RemoveMemberAsync(groupId, userId);

            return NoContent();
        }
    }
}