using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class GroupMasterService : IGroupMasterService
    {
        readonly BASS_DBContext _dbContext = new();

        public GroupMasterService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<GroupMaster> CreateGroupMaster(GroupMaster newGroupMaster)
        {
            try
            {
                var result = await this._dbContext.GroupMasters.AddAsync(newGroupMaster);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteGroupMaster(int grpid)
        {
            try
            {
                GroupMaster? grp = _dbContext.GroupMasters.Find(grpid);

                if (grp != null)
                {
                    _dbContext.GroupMasters.Remove(grp);
                    await _dbContext.SaveChangesAsync();
                    return "Success";
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            catch (Exception ex)
            {
                string GetError = ex.Message;
                return "ERROR";
            }
        }

        public async Task<List<string>> GetDDGroupMasters()
        {
            return await _dbContext.GroupMasters.Select(g=>g.GrpDesc).ToListAsync();
        }

        public async Task<GroupMaster> GetGroupMaster(int grpid)
        {
            try
            {
                GroupMaster? grp = await _dbContext.GroupMasters.Where(x => x.GrpId == grpid).FirstOrDefaultAsync();

                if (grp != null)
                {
                    return grp;
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<GroupMaster>> GetGroupMasters()
        {
            try
            {
                return await _dbContext.GroupMasters.ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> UpdateGroupMaster(GroupMaster updatedGroupMaster)
        {
            try
            {
                GroupMaster? gm1 = await _dbContext.GroupMasters.Where(x => x.GrpId == updatedGroupMaster.GrpId).FirstOrDefaultAsync();
                if (gm1 != null)
                {
                    gm1.GrpNo = updatedGroupMaster.GrpNo;
                    gm1.GrpDesc = updatedGroupMaster.GrpDesc;
                    gm1.GrpShortDesc = updatedGroupMaster.GrpShortDesc;
                    _dbContext.GroupMasters.Update(gm1);
                    await _dbContext.SaveChangesAsync();
                }
                return "Success";
            }
            catch (Exception ex)
            {
                string GetError = ex.Message;
                return "ERROR";
            }
        }
    }
}
