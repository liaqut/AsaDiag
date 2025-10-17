using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IGroupMasterService
    {
        public Task<List<GroupMaster>> GetGroupMasters();
        public Task<GroupMaster> GetGroupMaster(int grpid);
        public Task<GroupMaster> CreateGroupMaster(GroupMaster newGroupMaster);
        public Task<string> UpdateGroupMaster(GroupMaster updatedGroupMaster);
        public Task<string> DeleteGroupMaster(int grpid);
        public Task<List<string>> GetDDGroupMasters();
    }
}
