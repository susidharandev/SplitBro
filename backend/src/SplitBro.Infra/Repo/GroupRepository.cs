using Microsoft.EntityFrameworkCore;
using SplitBro.Application.InfraInterfaces;
using SplitBro.Domain;
using SplitBro.Infra.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitBro.Infra.Repo
{
    public class GroupRepository : IGroupRepository
    {
        private readonly SplitAppDbContext _context;
        public GroupRepository(SplitAppDbContext context)
        {
            _context = context;
        }
        // Group Repository Methods
        public async Task<Group> CreateGroupWithMemberAsync(Group group, GroupMember groupMember)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Groups.Add(group);

                await _context.SaveChangesAsync();

                groupMember.GroupId = group.Id;

                _context.GroupMembers.Add(groupMember);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return group;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<List<Group>> GetGroupAllAsync()
        {
            return await _context.Groups.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
        }
        public async Task<Group?> GetGroupByIdAsync(int groupId)
        {
            return await _context.Groups.FirstOrDefaultAsync(x=> x.Id == groupId);
        }
        public async Task UpdateGroupAsync(Group group)
        {
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
        }

        // Group Member Repository Methods
        public async Task<GroupMember> AddMemberAsync(GroupMember groupMember)
        {
            _context.GroupMembers.Add(groupMember);
            await _context.SaveChangesAsync();

            return groupMember;
        }
        public async Task<List<GroupMember>> GetMembersAsync(int groupId)
        {
            return await _context.GroupMembers
                        .Include(x => x.User) // N+1 query problem:
                        .Where(x => x.GroupId == groupId)
                        .ToListAsync();
        }
        public async Task<GroupMember?> GetMemberAsync(int groupId, int userId)
        {
            return await _context.GroupMembers.FirstOrDefaultAsync(x => x.GroupId == groupId && x.UserId == userId);
        }
        public async Task<int> GetMemberCountAsync(int groupId)
        {
            return await _context.GroupMembers.CountAsync(x => x.GroupId == groupId);
        }
        public async Task RemoveMemberAsync(GroupMember groupMember)
        {
            _context.GroupMembers.Remove(groupMember);
            await _context.SaveChangesAsync();
        }
    }
}
