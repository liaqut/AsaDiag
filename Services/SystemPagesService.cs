using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class SystemPagesService : ISystemPagesService
    {
        readonly BASS_DBContext _dbContext = new();

        public SystemPagesService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<SystemPage>> GetSystemPages(string vComp)
        {
            try
            {
                return await _dbContext.SystemPages.Where(x=>x.PageCompType==vComp).OrderBy(e => e.PageId).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
