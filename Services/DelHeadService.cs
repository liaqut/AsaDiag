using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Numerics;
using static DigiEquipSys.Pages.Del_pg;

namespace DigiEquipSys.Services
{
    public class DelHeadService : IDelHeadService
    {
        readonly BASS_DBContext _dbContext = new();

        public DelHeadService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<DelHead> CreateDelHead(DelHead newDelHead)
        {
            try
            {
                var result = await this._dbContext.DelHeads.AddAsync(newDelHead);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<BigInteger> GetNextDelNoAsync()
        {
            var connection = _dbContext.Database.GetDbConnection();
            await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "GetNextDelNo";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                var result = await command.ExecuteScalarAsync();
                await connection.CloseAsync();
                if (result is System.Numerics.BigInteger bigIntResult)
                {
                    return (int)bigIntResult;
                }
                else
                {
                    return Convert.ToInt32(result);
                }
            }
        }
        public async Task<string> DeleteDelHead(long Delheadid)
        {
            try
            {
                DelHead? th = await _dbContext.DelHeads.FindAsync(Delheadid);

                if (th != null)
                {
                    _dbContext.DelHeads.Remove(th);
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

        public async Task<DelHead> GetDelHead(long Delheadid)
        {
            try
            {
                DelHead?  th = await _dbContext.DelHeads.Where(x => x.DelId == Delheadid).AsNoTracking().FirstOrDefaultAsync();

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

        public async Task<DelHead> GetDelHeadByDelNumber(long Delnum)
        {
            try
            {
                DelHead? th = await _dbContext.DelHeads.Where(x => x.DelNo == Delnum).AsNoTracking().FirstOrDefaultAsync();

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

        public async Task<DelHead> GetDelHeadByDispNo(string dispnum)
        {
            try
            {
                DelHead? th9 = await _dbContext.DelHeads.Where(x => x.DelDispNo == dispnum && x.DelApproved==true).AsNoTracking().FirstOrDefaultAsync();

                if (th9 != null)
                {
                    return th9;
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

        public async Task<List<DelHeadResult>> GetDelHeadResults()
        {
            try
            {
                var query = await (from delHead in _dbContext.DelHeads
                                   join delDetl in _dbContext.DelDetls on delHead.DelId equals delDetl.DelHeadId
                                   group new { delHead, delDetl } by new
                                   {
                                       delHead.DelId,
                                       delHead.DelDispNo,
                                       delHead.DelDate,
                                       delHead.DelClientId,
                                       delHead.DelCityId,
                                       delHead.PoNumber,
                                       delHead.DelApproved
                                   } into g
                                   select new DelHeadResult
                                   {
                                       DelId = (long)g.Key.DelId,
                                       DelDispNo = g.Key.DelDispNo ?? string.Empty,
                                       DelDate = (DateTime)g.Key.DelDate,
                                       DelClientId = (int)g.Key.DelClientId,
                                       DelCityId = (int)g.Key.DelCityId,
                                       PoNumber = g.Key.PoNumber ?? string.Empty,
                                       DelApproved = (bool)g.Key.DelApproved,
                                       DiffAmt = (decimal)g.Sum(x => (x.delDetl.DelUprice - x.delDetl.DelPurchPrice) * x.delDetl.DelQty),
                                       Margin = (decimal)(g.Sum(x => x.delDetl.DelQty * x.delDetl.DelUprice) == 0 ?
                                                -100 :
                                                ((g.Sum(x => x.delDetl.DelQty * x.delDetl.DelUprice) - g.Sum(x => x.delDetl.DelQty * x.delDetl.DelPurchPrice)) /
                                                 g.Sum(x => x.delDetl.DelQty * x.delDetl.DelUprice)) * 100)
                                   }).OrderByDescending(y => y.DelDispNo).AsNoTracking().ToListAsync();
                return query;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<DelHead>> GetDelHeads()
        {
            try
            {
                return await _dbContext.DelHeads.OrderByDescending(y=>y.DelNo).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<DelHead>> GetDelHeads(string loc)
        {
            return await _dbContext.DelHeads.OrderByDescending(y=>y.DelNo).AsNoTracking().ToListAsync();
        }

        //public async Task<List<DelSale>> GetDelHeadSale()
        //{
            //var vresult = await (from x in _dbContext.DelHeads join y in _dbContext.SdelHeads on x.DelDispNo equals y.DelDispNo into w from y in w.DefaultIfEmpty()
            //                     select new DelSale { DelDispNo=x.DelDispNo, DelDate=x.DelDate, DelClientId = x.DelClientId, SDelInvNo = y.SdelInvNo }).OrderByDescending(t=>t.DelDispNo).ToListAsync();
            //return vresult;

        //}

        public async Task<string> UpdateDelHead(DelHead updatedDelHead)
        {
            try
            {
                DelHead? th1 = await _dbContext.DelHeads.Where(x => x.DelId == updatedDelHead.DelId).FirstOrDefaultAsync();
                if (th1 != null)
                {
                    th1.DelNo = updatedDelHead.DelNo;
                    th1.DelDate = updatedDelHead.DelDate;
                    th1.PoNumber = updatedDelHead.PoNumber;
                    th1.PoDate = updatedDelHead.PoDate;
                    th1.DelCityId = updatedDelHead.DelCityId;
                    th1.DelDispNo = updatedDelHead.DelDispNo;
                    th1.DelClientId = updatedDelHead.DelClientId;
                    th1.DelUser = updatedDelHead.DelUser;
                    th1.DelDateAltered = updatedDelHead.DelDateAltered;
                    th1.DelApproved = updatedDelHead.DelApproved;
                    th1.DelComp = updatedDelHead.DelComp;
                    th1.DelBranch = updatedDelHead.DelBranch;
                    _dbContext.DelHeads.Update(th1);
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
