using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace DigiEquipSys.Services
{
    public class SDelHeadService : ISDelHeadService
    {
        readonly BASS_DBContext _dbContext = new();

        public SDelHeadService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<SdelHead> CreateSdelHead(SdelHead newSdelHead)
        {
            try
            {
                var result = await this._dbContext.SdelHeads.AddAsync(newSdelHead);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<BigInteger> GetNextSdelNoAsync()
        {
            var connection = _dbContext.Database.GetDbConnection();
            await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "GetNextSdelNo";
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
        public async Task<string> DeleteSdelHead(long SdelHeadid)
        {
            try
            {
                SdelHead? th = await _dbContext.SdelHeads.FindAsync(SdelHeadid);

                if (th != null)
                {
                    _dbContext.SdelHeads.Remove(th);
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

        public async Task<SdelHead> GetSdelHead(long SdelHeadid)
        {
            try
            {
                SdelHead? th = await _dbContext.SdelHeads.Where(x => x.SdelId == SdelHeadid).AsNoTracking().FirstOrDefaultAsync();

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

        public async Task<SdelHead> GetSdelHead(DateTime StDate,DateTime EnDate)
        {
            try
            {
                SdelHead? th = await _dbContext.SdelHeads.Where(x => x.SdelDateFrom==StDate && x.SdelDateTo==EnDate).AsNoTracking().FirstOrDefaultAsync();

                if (th != null)
                {
                    return th;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<SdelHead> GetSdelHeadByDelDispNo(string Deldispnum)
        {
            try
            {
                SdelHead? th9 = await _dbContext.SdelHeads.Where(x => x.SdelDispNo == Deldispnum && x.SdelApproved == true).AsNoTracking().FirstOrDefaultAsync();

                if (th9 != null)
                {
                    return th9;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<SdelHead> GetSdelHeadAllByDelDispNo(string Deldispnum)
        {
            try
            {
                SdelHead? th9 = await _dbContext.SdelHeads.Where(x => x.SdelDispNo == Deldispnum ).AsNoTracking().FirstOrDefaultAsync();

                if (th9 != null)
                {
                    return th9;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<SdelHead> GetSdelHeadBySDelNumber(long sDelnum)
        {
            try
            {
                SdelHead? th = await _dbContext.SdelHeads.Where(x => x.SdelNo == sDelnum).AsNoTracking().FirstOrDefaultAsync();

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

        public async Task<List<SdelHead>> GetSdelHeads()
        {
            try
            {
                return await _dbContext.SdelHeads.OrderByDescending(y => y.SdelNo).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> UpdateSdelHead(SdelHead updateSdelHead)
        {
            try
            {
                SdelHead? th1 = await _dbContext.SdelHeads.Where(x => x.SdelId == updateSdelHead.SdelId).FirstOrDefaultAsync();
                if (th1 != null)
                {
                    th1.SdelNo = updateSdelHead.SdelNo;
                    th1.SdelDate = updateSdelHead.SdelDate;
                    th1.SdelDispNo = updateSdelHead.SdelDispNo;
                    th1.SdelUser = updateSdelHead.SdelUser;
                    th1.SdelDateAltered = updateSdelHead.SdelDateAltered;
                    th1.SdelApproved = updateSdelHead.SdelApproved;
                    th1.SdelDateFrom = updateSdelHead.SdelDateFrom;
                    th1.SdelDateTo = updateSdelHead.SdelDateTo;
                    th1.SdelComp = updateSdelHead.SdelComp;
                    th1.SdelBranch = updateSdelHead.SdelBranch;
                    _dbContext.SdelHeads.Update(th1);
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
