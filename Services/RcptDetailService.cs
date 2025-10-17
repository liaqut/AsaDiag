using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class RcptDetailService : IRcptDetailService
    {
        readonly BASS_DBContext _dbContext = new();

        public RcptDetailService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<RcptDetail> AddRcptDetail(RcptDetail newrcpt)
        {
            try
            {
                var result = await this._dbContext.RcptDetails.AddAsync(newrcpt);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteRcptDetail(long id)
        {
            try
            {
                var rdet = await _dbContext.RcptDetails.FindAsync(id);

                if (rdet == null)
                {
                    return false;
                }
                else
                {
                    _dbContext.RcptDetails.Remove(rdet);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                string GetError = ex.Message;
                return false;
            }
        }

        
        public async Task<string> DeleteRcptDetailbyRcptHead(long rcptheadId)
        {
            try
            {
                List<RcptDetail>? rd =  _dbContext.RcptDetails.Where(x=>x.RdRhId==rcptheadId).AsNoTracking().ToList();

                if (rd != null)
                {
                    foreach (var vRec in rd)
                    {
                        _dbContext.RcptDetails.Remove(vRec);
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


        public async Task<RcptDetail> GetRcptDetail(long id)
        {
            try
            {
                RcptDetail? rd = await _dbContext.RcptDetails.Where(x => x.RdId == id).AsNoTracking().FirstOrDefaultAsync();

                if (rd != null)
                {
                    return rd;
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

        public async Task<List<RcptDetail>> GetRcptDetails()
        {
            try
            {
                return await _dbContext.RcptDetails.AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<RcptDetail>> GetRcptDetails(string listNo,string clcode, string pohno)
        {
            try
            {
                return await _dbContext.RcptDetails.Where(y=>y.RdListNo==listNo && y.RdClientCode==clcode && y.RdPohDispNo==pohno).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<RcptDetail>> GetRcptDetails(long rcptserno)
        {
            try
            {
                return await _dbContext.RcptDetails.Where(x => x.RdRhId == rcptserno || x.RdRhId==null).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }


        public async Task<double> GetTotRcptQtyByPo(long pono, long stkid)
        {
            try
            {
                var th = await (from rd in _dbContext.RcptDetails
                                join rh in _dbContext.RcptHeads on rd.RdRhId equals rh.RhId
                                where rd.RdStkId == stkid
                                group new { rd, rh } by rd.RdStkId into v
                                select new
                                {
                                    qty = Convert.ToDouble(v.Sum(x => x.rd.RdQty ?? 0))
                                }).AsNoTracking().FirstOrDefaultAsync();
                return Convert.ToDouble(th?.qty ?? 0);

            }
            catch
            {
                throw;
            }
        }

        public async Task<(double,double)> GetTotRcptQtyByStkId(string whno, long stkid)
        {
            
            try
            {
                var th = await (from rd in _dbContext.RcptDetails
                                join rh in _dbContext.RcptHeads on rd.RdRhId equals rh.RhId
                                where rd.RdStkId == stkid
                                group new { rd, rh } by rd.RdStkId into v
                                select new
                                {
                                    qty = Convert.ToDouble(v.Sum(x => x.rd.RdQty ?? 0)),
                                    amt = Convert.ToDouble(v.Sum(x => (x.rd.RdQty ?? 0) * (x.rd.RdUp ?? 0)))

                                }).AsNoTracking().FirstOrDefaultAsync();

                return (Convert.ToDouble(th?.qty ?? 0),Convert.ToDouble(th?.amt ?? 0));

            } 
            catch
            {
                throw;
            }
        }


        public async Task<string> UpdateRcptDetail(RcptDetail rcpt)
        {
            try
            {
                RcptDetail? th1 = await _dbContext.RcptDetails.Where(x => x.RdId == rcpt.RdId).FirstOrDefaultAsync();
                if (th1 != null)
                {
                    th1.RdRhId = rcpt.RdRhId;
                    th1.RdScanCode= rcpt.RdScanCode;
                    th1.RdListNo= rcpt.RdListNo;
                    th1.RdLotNo= rcpt.RdLotNo;
                    th1.RdExpiryDate= rcpt.RdExpiryDate;
                    th1.RdUp = rcpt.RdUp;
                    th1.RdQty = rcpt.RdQty;
                    th1.RdStkIdDesc = rcpt.RdStkIdDesc;
                    th1.RdStkIdGrp = rcpt.RdStkIdGrp;
                    th1.RdStkIdCat= rcpt.RdStkIdCat;
                    th1.RdStkIdUnit = rcpt.RdStkIdUnit;
                    th1.RdStkId = rcpt.RdStkId;
                    th1.RdStockStkId = rcpt.RdStockStkId;
                    th1.RdSuppCode= rcpt.RdSuppCode;
                    th1.RdPohDispNo= rcpt.RdPohDispNo;
                    th1.RdClientCode= rcpt.RdClientCode;
                    th1.RdVendInvNo= rcpt.RdVendInvNo;
                    th1.RdVendInvDate= rcpt.RdVendInvDate;
                    _dbContext.RcptDetails.Update(th1);
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
