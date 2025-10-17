using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class SysPagesControlService : ISysPagesControlService
    {
        readonly BASS_DBContext _dbContext = new();

        public SysPagesControlService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SysPagesControl> CreateSysPagesControl(SysPagesControl newSysPagesControl)
        {
            try
            {
                var result = await this._dbContext.SysPagesControls.AddAsync(newSysPagesControl);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteSysPagesControl(int syspagesid)
        {
            try
            {
                SysPagesControl? sysp = _dbContext.SysPagesControls.Find(syspagesid);

                if (sysp != null)
                {
                    _dbContext.SysPagesControls.Remove(sysp);
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
        public async Task<string> DeleteSysPagesControl(string email)
        {
            try
            {
                var result = await _dbContext.SysPagesControls.Where (x=>x.SysPagesEmail == email).ToListAsync();
                foreach(var x in result)
                {
                    _dbContext.SysPagesControls.Remove(x);
                }
                await _dbContext.SaveChangesAsync();
                return "Success";
            }
            catch (Exception ex)
            {
                string GetError = ex.Message;
                return "ERROR";
            }
        }


        public Task<SysPagesControl> GetSysPagesControl(int syspagesid)
        {
            throw new NotImplementedException();
        }

        public async Task<List<SysPagesControl>> GetSysPagesControls()
        {
            try
            {
                return await _dbContext.SysPagesControls.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<SysPagesControl>> GetSysPagesControls(string mailId,string vComp)
        {
            try
            {
                return await (from x in _dbContext.SysPagesControls join y in _dbContext.SystemPages on x.SysPagesControlId equals y.PageId orderby x.SysPagesControlId where x.SysPagesEmail == mailId && y.PageCompType==vComp  select x ).ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<SysPagesControl?> GetSysPagesControls(string mailId, int ctrlid)
        {
            try
            {
                return await _dbContext.SysPagesControls.Where(x => x.SysPagesEmail == mailId && x.SysPagesControlId == ctrlid && x.SysPagesAuthorized==true).FirstOrDefaultAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> UpdateSysPagesControl(SysPagesControl updatedSysPagesControl)
        {
            try
            {
                SysPagesControl? pc1 = await _dbContext.SysPagesControls.Where(x => x.SysPagesId == updatedSysPagesControl.SysPagesId).FirstOrDefaultAsync();
                if (pc1 != null)
                {
                    pc1.SysPagesControlId = updatedSysPagesControl.SysPagesControlId;
                    pc1.SysPagesEmail = updatedSysPagesControl.SysPagesEmail;
                    pc1.SysPagesAuthorized = updatedSysPagesControl.SysPagesAuthorized;
                    _dbContext.SysPagesControls.Update(pc1);
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
