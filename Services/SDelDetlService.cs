using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DigiEquipSys.Services
{
    public class SDelDetlService : ISDelDetlService
    {
        readonly BASS_DBContext _dbContext = new();
        public SDelDetlService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SdelDetl> CreateSDelDetl(SdelDetl newSDelDetl)
        {
            try
            {
                var result = await this._dbContext.SdelDetls.AddAsync(newSDelDetl);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteSDelDetl(long sDeldetlid)
        {
            try
            {
                SdelDetl? th = await _dbContext.SdelDetls.FindAsync(sDeldetlid);

                if (th != null)
                {
                    _dbContext.SdelDetls.Remove(th);
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

        public async Task DeleteBySdelHeadId(long ids)
        {
            try
            {
                var entitiesToDelete = await _dbContext.SdelDetls.Where(e => e.SdelHeadId == ids).AsNoTracking().ToListAsync();
                _dbContext.SdelDetls.RemoveRange(entitiesToDelete);
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<SdelDetl> GetSDelDetl(long sDeldetlid)
        {
            try
            {
                SdelDetl? th = await _dbContext.SdelDetls.Where(x => x.SdelDetId == sDeldetlid).AsNoTracking().FirstOrDefaultAsync();

                if (th != null)
                {
                    return th;
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

        public async Task<List<SdelDetl>> GetSDelDetls()
        {
            try
            {
                return await _dbContext.SdelDetls.AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<SdelDetl>> GetSDelDetlsBySdelHeadId(long SdelHeadid)
        {
            try
            {
                return await _dbContext.SdelDetls.Where(x => x.SdelHeadId== SdelHeadid).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> UpdateSDelDetl(SdelDetl updateSDelDetl)
        {
            try
            {
                SdelDetl? th1 = await _dbContext.SdelDetls.Where(x => x.SdelDetId == updateSDelDetl.SdelDetId).FirstOrDefaultAsync();
                if (th1 != null)
                {
                    th1.SdelHeadId = updateSDelDetl.SdelHeadId;
                    th1.SdelDate = updateSDelDetl.SdelDate;
                    th1.SdelListNo = updateSDelDetl.SdelListNo;
                    th1.SdelClientCode = updateSDelDetl.SdelClientCode;
                    th1.SdelClientVendCode = updateSDelDetl.SdelClientVendCode;
                    th1.SdelLotNo = updateSDelDetl.SdelLotNo;
                    th1.SdelStkIdDesc = updateSDelDetl.SdelStkIdDesc;
                    th1.SdelExpiryDate = updateSDelDetl.SdelExpiryDate;
                    th1.SdelProdCode = updateSDelDetl.SdelProdCode;
                    th1.SdelListNoProd = updateSDelDetl.SdelListNoProd;
                    th1.SdelQty = updateSDelDetl.SdelQty;
                    th1.SdelUprice = updateSDelDetl.SdelUprice;
                    _dbContext.SdelDetls.Update(th1);
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
