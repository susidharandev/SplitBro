using SplitBro.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBro.Application.InfraInterfaces
{
    public interface IGroupRepository
    {
        Task<Group> CreateGroupWithMemberAsync(Group group, GroupMember groupMember);
        Task<Group?> GetGroupByIdAsync(int groupId);
        Task<List<Group>> GetGroupAllAsync();
        Task UpdateGroupAsync(Group group);


        Task<GroupMember> AddMemberAsync(GroupMember groupMember);
        Task<GroupMember?> GetMemberAsync(int groupId, int userId);
        Task<List<GroupMember>> GetMembersAsync(int groupId);
        Task<int> GetMemberCountAsync(int groupId);
        Task RemoveMemberAsync(GroupMember groupMember);

        // DeleteGroup

    }
}
