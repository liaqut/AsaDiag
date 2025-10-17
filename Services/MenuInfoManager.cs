using DigiEquipSys.Models;
using DigiEquipSys.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class MenuInfoManager : IMenuInfo
    {
        readonly BASS_DBContext _dbContext = new();

        public MenuInfoManager(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        //To Get all user details   
        public async Task<List<MenuInfo>> GetMenuDetails()
        {
            try
            {
                return await _dbContext.MenuInfos.ToListAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}