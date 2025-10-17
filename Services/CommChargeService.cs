using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class CommChargeService : ICommChargeService
    {
        readonly BASS_DBContext _dbContext = new();
        public CommChargeService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<CommCharge> CreateCommCharge(CommCharge newCommCharge)
        {
            try
            {
                var result = await this._dbContext.CommCharges.AddAsync(newCommCharge);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteCommCharge(long commid)
        {
            try
            {
                CommCharge? im = _dbContext.CommCharges.Find(commid);

                if (im != null)
                {
                    _dbContext.CommCharges.Remove(im);
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

        public async Task<CommCharge> GetCommCharge(long commid)
        {
            try
            {
                CommCharge? im = await _dbContext.CommCharges.Where(x => x.CommId == commid).AsNoTracking().FirstOrDefaultAsync();

                if (im != null)
                {
                    return im;
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

        public async Task<List<CommCharge>> GetCommCharges()
        {
            try
            {
                return await _dbContext.CommCharges.AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

		public async Task<List<CommCharge>> GetCommChargeDate(DateTime vStDate, DateTime vEnDate)
		{
			try
			{
				var start = vStDate.Date;
				var endExclusive = vEnDate.Date;
				return await _dbContext.CommCharges.Where(x => x.CommDate >= start && x.CommDate < endExclusive).OrderBy(z => z.CommDate).AsNoTracking().ToListAsync();
			}
			catch
			{
				throw;
			}
		}

		public async Task<string> UpdateCommCharge(CommCharge updatedCommCharge)
        {
            try
            {
                CommCharge? th1 = await _dbContext.CommCharges.Where(x => x.CommId == updatedCommCharge.CommId).FirstOrDefaultAsync();
                if (th1 != null)
                {
                    th1.CommDate = updatedCommCharge.CommDate;
                    th1.CommDesc = updatedCommCharge.CommDesc;
                    th1.CommAmt = updatedCommCharge.CommAmt;
                    th1.CommRemarks = updatedCommCharge.CommRemarks;
                    _dbContext.CommCharges.Update(th1);
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
