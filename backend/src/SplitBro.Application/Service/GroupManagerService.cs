using SplitBro.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SplitBro.Application.InfraInterfaces;
using static SplitBro.Application.Dto.GroupDto;

namespace SplitBro.Application.Service
{
    public class GroupManagerService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IUserRepository _userRepository;

        private readonly ILogger<GroupManagerService> _logger;

        public GroupManagerService(IGroupRepository groupRepository, IUserRepository userRepository, ILogger<GroupManagerService> logger)
        {
            _groupRepository = groupRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        // Group Management Methods
        public async Task<GroupResponse> CreateGroupWithMemberAsync(CreateGroupRequest createGroupRequest)
        {
            if (createGroupRequest == null)
                throw new ArgumentNullException("Group data is empty");

            var user = await _userRepository.GetByIdAsync(createGroupRequest.UserId);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            var group = new Group
            {
                Name = createGroupRequest.Name,
                Description = createGroupRequest.Description,
                SimplifyDebt = createGroupRequest.SimplifyDebt
            };

            var groupMember = new GroupMember
            {
                UserId = user.Id
            };

            var newGroup = await _groupRepository.CreateGroupWithMemberAsync(group, groupMember);

            _logger.LogInformation("Created group for user {UserId}", createGroupRequest.UserId);

            return ToGroupResponse(newGroup);
        }
        public async Task<List<GroupResponse>> GetGroupAllAsync()
        {
            var groups = await _groupRepository.GetGroupAllAsync();

            if (groups.Count == 0)
            {
                _logger.LogInformation("No Groups found");
            }

            return groups.Select(group => ToGroupResponse(group)).ToList();
        }
        public async Task<GroupResponse> GetGroupByIdAsync (int groupId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);

            if (group == null)
            {
                _logger.LogInformation("Group with ID {Id} not found", groupId);
                throw new KeyNotFoundException("Group not found");
            }

            return ToGroupResponse(group);
        }
        public async Task<GroupResponse> UpdateGroupAsync(int groupId, UpdateGroupRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("Group data is empty");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Group name is required.");

            var group = await _groupRepository.GetGroupByIdAsync(groupId);

            if (group == null)
                throw new KeyNotFoundException("Group not found");

            group.Name = request.Name;
            group.Description = request.Description;
            group.SimplifyDebt = request.SimplifyDebt;
            group.LastUpdatedAt = DateTime.UtcNow;

            await _groupRepository.UpdateGroupAsync(group);

            _logger.LogInformation("Group Details Updated for Group Id {GroupId}", groupId);

            return ToGroupResponse(group);
        }

        // Group Member Management Methods
        public async Task<MemberResponse> AddMemberAsync(int groupId, int userId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);

            if (group == null)
                throw new KeyNotFoundException("Group not found");

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            var existingMember = await _groupRepository.GetMemberAsync(groupId, userId);

            if (existingMember != null)
                throw new InvalidOperationException("User is already a member of this group.");

            var groupMember = new GroupMember
            {
                GroupId = groupId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            };

            var newMember = await _groupRepository.AddMemberAsync(groupMember);

            _logger.LogInformation("Member {UserId} added to group {GroupId}", userId, groupId);

            return new MemberResponse
            {
                UserId = userId,
                GroupId = groupId,
                GroupName = group.Name,
                JoinedAt = newMember.JoinedAt
            };
        }
      
        public async Task<List<MemberResponse>> GetMembersAsync(int groupId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);

            if (group == null)
            {
                _logger.LogInformation("Group Id {GroupId} not found", groupId);
                throw new KeyNotFoundException("Group not found");
            }

            var members = await _groupRepository.GetMembersAsync(groupId);

            return members.Select(member => new MemberResponse
            {
                UserId = member.UserId,
                GroupId = member.GroupId,
                GroupName = group.Name,
                JoinedAt = member.JoinedAt
            }).ToList();
        }
        public async Task RemoveMemberAsync(int groupId, int userId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);

            if (group == null)
                throw new KeyNotFoundException("Group not found");

            var member = await _groupRepository.GetMemberAsync(groupId, userId);

            if (member == null)
                throw new KeyNotFoundException("User is not a member of this group");

            var memberCount = await _groupRepository.GetMemberCountAsync(groupId);

            if (memberCount <= 1)
                throw new InvalidOperationException("A group must have at least one member.");

            await _groupRepository.RemoveMemberAsync(member);

            _logger.LogInformation("Member Removed from group {GroupId}", groupId);
        }
        private static GroupResponse ToGroupResponse(Group group)
        {
            return new GroupResponse
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                CreatedAt = group.CreatedAt
            };
        }
    }
}
