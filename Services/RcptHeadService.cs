using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace DigiEquipSys.Services
{
    public class RcptHeadService : IRcptHeadService
    {
        readonly BASS_DBContext _dbContext = new();

        public RcptHeadService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<BigInteger> GetNextRhNoAsync()
        {
            var connection = _dbContext.Database.GetDbConnection();
            await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "GetNextRhNo";
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

        public async Task<RcptHead> AddRcptHead(RcptHead rcpt)
        {
            try
            {
                var result = await this._dbContext.RcptHeads.AddAsync(rcpt);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteRcptHead(long id)
        {
            try
            {
                RcptHead? th = _dbContext.RcptHeads.Find(id);

                if (th != null)
                {
                    _dbContext.RcptHeads.Remove(th);
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

        public async Task<RcptHead> GetRcptHead(long id)
        {
            try
            {
                RcptHead? th = await _dbContext.RcptHeads.Where(x => x.RhId == id).AsNoTracking().FirstOrDefaultAsync();

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
        //public async Task<RcptHead> GetRcptHeadbyPoInv(long poinvno)
        //{
        //    try
        //    {
        //        RcptHead? th = await _dbContext.RcptHeads.Where(x => x.RhPoInvNo == poinvno).FirstOrDefaultAsync();

        //        if (th != null)
        //        {
        //            return th;
        //        }
        //        else
        //        {
        //            throw new ArgumentNullException();
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public async Task<RcptHead> GetRcptHeadbyRcptNo(long rcptno)
        {
            try
            {
                RcptHead? th = await _dbContext.RcptHeads.Where(x => x.RhNo == rcptno).AsNoTracking().FirstOrDefaultAsync();

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
        public async Task<List<RcptHead>> GetRcptHeads()
        {
            try
            {
                return await _dbContext.RcptHeads.OrderByDescending(x=>x.RhNo).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }



        public async Task<List<RcptHead>> GetRcptHeads(string vWhcode)
        {
            try
            {
                return await _dbContext.RcptHeads.AsNoTracking().ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public async Task<List<RcptHead>> GetRcptHeadsByPo(long PoNo, string loc)
        //{
        //    try
        //    {
        //        return await _dbContext.RcptHeads.Where(x => x.RhPoNo == PoNo && x.RhWhCode==loc).ToListAsync();
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public async Task<string> UpdateRcptHead(RcptHead rcpt)
        {
            try
            {
                RcptHead? th1 = await _dbContext.RcptHeads.Where(x => x.RhId == rcpt.RhId).FirstOrDefaultAsync();
                if (th1 != null)
                {
                    th1.RhNo = rcpt.RhNo;
                    th1.RhDispNo = rcpt.RhDispNo;
                    th1.RhDate = rcpt.RhDate;
                    th1.RhRemarks = rcpt.RhRemarks;
                    th1.RhUser = rcpt.RhUser;
                    th1.RhDateAltered = rcpt.RhDateAltered;
                    th1.RhApproved=rcpt.RhApproved;
                    th1.RhPoNo = rcpt.RhPoNo;
                    th1.RhSuppId=rcpt.RhSuppId;
                    th1.RhVendInvNote = rcpt.RhVendInvNote;
                    th1.RhVendDelNote = rcpt.RhVendDelNote;
                    th1.RhComp = rcpt.RhComp;
                    th1.RhBranch = rcpt.RhBranch;
                    _dbContext.RcptHeads.Update(th1);
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
