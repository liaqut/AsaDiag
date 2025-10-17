using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class CatMasterService : ICatMasterService
    {
        readonly BASS_DBContext _dbContext = new();

        public CatMasterService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<CategMaster> CreateCategMaster(CategMaster newCategMaster)
        {
            try
            {
                var result = await this._dbContext.CategMasters.AddAsync(newCategMaster);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteCategMaster(int catid)
        {
            try
            {
                CategMaster? cat = _dbContext.CategMasters.Find(catid);

                if (cat != null)
                {
                    _dbContext.CategMasters.Remove(cat);
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

        public async Task<CategMaster> GetCategMaster(int catid)
        {
            try
            {
                CategMaster? categ = await _dbContext.CategMasters.Where(x => x.CatId == catid).FirstOrDefaultAsync();

                if (categ != null)
                {
                    return categ;
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

        public async Task<List<CategMaster>> GetCategMasters(string grpno)
        {
            return await _dbContext.CategMasters.Where(t=>t.CatGrpNo==grpno).ToListAsync();
        }
        public async Task<List<CategMaster>> GetCategMasters()
        {
            return await _dbContext.CategMasters.ToListAsync();
        }

        public async Task<List<string>> GetDDCategMasters()
        {
            return await _dbContext.CategMasters.Select(g => g.CatDesc).ToListAsync();
        }

        public async Task<string> UpdateCategMaster(CategMaster updatedCategMaster)
        {
            try
            {
                CategMaster? gm1 = await _dbContext.CategMasters.Where(x => x.CatId == updatedCategMaster.CatId).FirstOrDefaultAsync();
                if (gm1 != null)
                {
                    gm1.CatNo = updatedCategMaster.CatNo;
                    gm1.CatGrpNo = updatedCategMaster.CatGrpNo;
                    gm1.CatDesc = updatedCategMaster.CatDesc;
                    gm1.CatShortDesc = updatedCategMaster.CatShortDesc;
                    _dbContext.CategMasters.Update(gm1);
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
