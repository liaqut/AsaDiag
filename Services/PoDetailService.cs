using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace DigiEquipSys.Services
{
    public class PoDetailService : IPoDetailService
    {
        readonly BASS_DBContext _dbContext = new();

        public PoDetailService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<PoDetail> CreatePoDetail(PoDetail newPoDetail)
        {
            try
            {
                var result = await this._dbContext.PoDetails.AddAsync(newPoDetail);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeletePoDetail(long podetailid)
        {
            try
            {
                var th = await _dbContext.PoDetails.FindAsync(podetailid);

                if (th != null)
                {
                    _dbContext.PoDetails.Remove(th);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                string GetError = ex.Message;
                return false;
            }
        }

        public async Task<List<VwPurchaseOrder>> GetvwPoByDate(DateTime vStDate, DateTime vEnDate)
        {
            try
            {
				var start = vStDate.Date;
				var endExclusive = vEnDate.Date;
				return await _dbContext.VwPurchaseOrders.Where(x => x.PohDate >= start && x.PohDate < endExclusive).OrderBy(z => z.PohDate).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<VwPurchaseOrder>> GetvwPendingPo(DateTime vStDate, DateTime vEnDate)
        {
            try
            {
				var start = vStDate.Date;
				var endExclusive = vEnDate.Date;
				return await _dbContext.VwPurchaseOrders.Where(x => x.PoQty - x.PoRcvdQty>0 && x.PohDate >= start && x.PohDate < endExclusive).OrderBy(z => z.PohDate).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<VwPurchaseOrder>> GetvwExcessPo(DateTime vStDate, DateTime vEnDate)
        {
            try
			{
				var start = vStDate.Date;
				var endExclusive = vEnDate.Date;
				return await _dbContext.VwPurchaseOrders.Where(x => x.PoQty - x.PoRcvdQty < 0 && x.PohDate >= start && x.PohDate < endExclusive).OrderBy(z => z.PohDate).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<PoDetail> GetPoDetail(long podetailid)
        {
            try
            {
                PoDetail th = await _dbContext.PoDetails.Where(x => x.PodId == podetailid).FirstOrDefaultAsync();

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

        public async Task<PoDetail> GetPoDetail(long pono,long stkcode)
        {
            try
            {
                PoDetail th = await _dbContext.PoDetails.Where(x => x.PodPohId == pono && x.PodStkIdDesc==stkcode).FirstOrDefaultAsync();

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

        public async Task<PoDetail> GetPoDetail(long pono, string listno)
        {
            try
            {
                PoDetail th = await _dbContext.PoDetails.Where(x => x.PodPohId == pono && x.PodListNo == listno).FirstOrDefaultAsync();

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



        public async Task<List<PoDetail>> GetPoDetails()
        {
            try
            {
                return await _dbContext.PoDetails.ToListAsync();
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<PoDetail>> GetPoDetails(long pohid)
        {
            try
            {
                return await _dbContext.PoDetails.Where(x => x.PodPohId == pohid).ToListAsync();
            }
            catch
            {
                throw;
            }
        }
        public async Task<string> UpdatePoDetail(PoDetail updatedPoDetail)
        {
            try
            {
                PoDetail th1 = await _dbContext.PoDetails.Where(x => x.PodId == updatedPoDetail.PodId).FirstOrDefaultAsync();
                if (th1 != null)
                {
                    th1.PodPohId = updatedPoDetail.PodPohId;
                    th1.PodUp = updatedPoDetail.PodUp;
                    th1.PodQty = updatedPoDetail.PodQty;
                    th1.PodDiscPct = updatedPoDetail.PodDiscPct;
                    th1.PodDiscAmt = updatedPoDetail.PodDiscAmt;
                    th1.PodUpAftDisc = updatedPoDetail.PodUpAftDisc;
                    th1.PodGstPct = updatedPoDetail.PodGstPct;
                    th1.PodStkIdDesc = updatedPoDetail.PodStkIdDesc;
                    th1.PodStkIdGrp = updatedPoDetail.PodStkIdGrp;
                    th1.PodStkIdCat=updatedPoDetail.PodStkIdCat;
                    th1.PodStkIdUnit = updatedPoDetail.PodStkIdUnit;
                    th1.PodRcvdQty = updatedPoDetail.PodRcvdQty;
                    th1.PodInvdQty= updatedPoDetail.PodInvdQty;
                    th1.PodRtndQty= updatedPoDetail.PodRtndQty;
                    th1.PodAmount= updatedPoDetail.PodAmount;   
                    th1.PodListNo= updatedPoDetail.PodListNo;
                    th1.PodHsnCode= updatedPoDetail.PodHsnCode;
                }
                _dbContext.PoDetails.Update(th1);
                 await _dbContext.SaveChangesAsync();
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
