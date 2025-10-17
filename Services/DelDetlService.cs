using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class DelDetlService : IDelDetlService
    {
        readonly BASS_DBContext _dbContext = new();

        public DelDetlService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<DelDetl> CreateDelDetl(DelDetl newDelDetl)
        {
            try
            {
                var result = await this._dbContext.DelDetls.AddAsync(newDelDetl);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteDelDetl(long Deldetlid)
        {
            try
            {
                DelDetl? th = await _dbContext.DelDetls.FindAsync(Deldetlid);

                if (th != null)
                {
                    _dbContext.DelDetls.Remove(th);
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

        public async Task<string> DeleteDelDetlbyDelHead(long delheadId)
        {
            try
            {
                List<DelDetl>? rd = _dbContext.DelDetls.Where(x => x.DelHeadId == delheadId).ToList();

                if (rd != null)
                {
                    foreach (var vRec in rd)
                    {
                        _dbContext.DelDetls.Remove(vRec);
                    }
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


        public async Task<List<DelDetl>> GetDelDetlsByDelHeadId(long Delid)
        {
            try
            {
                return await _dbContext.DelDetls.Where(x => x.DelHeadId == Delid).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }
        public async Task<DelDetl> GetDelDetl(long Deldetlid)
        {
            try
            {
                DelDetl? th = await _dbContext.DelDetls.Where(x => x.DelDetId == Deldetlid).AsNoTracking().FirstOrDefaultAsync();

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

        public async Task<List<DelDetl>> GetDelDetls()
        {
            try
            {
                return await _dbContext.DelDetls.AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<DelDetl>> GetDelDetlsByDelNumber(long Delhno)
        {
            try
            {
                return await _dbContext.DelDetls.Where(x => x.DelHeadId == Delhno).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> UpdateDelDetl(DelDetl updatedDelDetl)
        {
            try
            {
                DelDetl? th1 = await _dbContext.DelDetls.Where(x => x.DelDetId == updatedDelDetl.DelDetId).FirstOrDefaultAsync();
                if (th1 != null)
                {
                    th1.DelScanCode = updatedDelDetl.DelScanCode;
                    th1.DelClientCode = updatedDelDetl.DelClientCode;
                    th1.DelHeadId = updatedDelDetl.DelHeadId;
                    th1.DelListNo = updatedDelDetl.DelListNo;
                    th1.DelLotNo = updatedDelDetl.DelLotNo;
                    th1.DelExpiryDate= updatedDelDetl.DelExpiryDate;    
                    th1.DelStkId = updatedDelDetl.DelStkId;
                    th1.DelQty = updatedDelDetl.DelQty;
                    th1.DelUprice = updatedDelDetl.DelUprice;
                    th1.DelStkIdDesc = updatedDelDetl.DelStkIdDesc;
                    th1.DelStkIdUnit = updatedDelDetl.DelStkIdUnit;
                    th1.DelStkIdGrp = updatedDelDetl.DelStkIdGrp;
                    th1.DelStkIdCat = updatedDelDetl.DelStkIdCat;
                    th1.DelStockStkId=updatedDelDetl.DelStockStkId;
                    th1.DelPurchPrice = updatedDelDetl.DelPurchPrice;
                    th1.DelBatchId = updatedDelDetl.DelBatchId;
                    _dbContext.DelDetls.Update(th1);
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
