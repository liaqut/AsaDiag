using DigiEquipSys.Models;
using DigiEquipSys.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class RoleService : IRoleService
    {
        readonly BASS_DBContext _dbContext = new();

        public RoleService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        //To Get all user details   
        public async Task<List<RoleInfo>> GetRoleDetails()
        {
            try
            {
                return await _dbContext.RoleInfos.ToListAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
