using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Numerics;

namespace DigiEquipSys.Services
{
    public class TrHeadService : ITrHeadService
    {
        readonly BASS_DBContext _dbContext = new();

        public TrHeadService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<TrHead> CreateTrHead(TrHead newTrHead)
        {
            try
            {
                var result = await this._dbContext.TrHeads.AddAsync(newTrHead);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BigInteger> GetNextTrNoAsync()
        {
            var connection = _dbContext.Database.GetDbConnection();
            await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "GetNextTrNo";
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

        public async Task<string> DeleteTrHead(long Trheadid)
        {
            try
            {
                TrHead? th = await _dbContext.TrHeads.FindAsync(Trheadid);

                if (th != null)
                {
                    _dbContext.TrHeads.Remove(th);
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

        public async Task<TrHead> GetTrHead(long Trheadid)
        {
            try
            {
                TrHead? th = await _dbContext.TrHeads.Where(x => x.TrhId == Trheadid).AsNoTracking().FirstOrDefaultAsync();

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

        public async Task<TrHead> GetTrHeadByDispNo(string Trdispnum)
        {
            try
            {
                TrHead? th9 = await _dbContext.TrHeads.Where(x => x.TrhDispNo== Trdispnum && x.TrhApproved == true).AsNoTracking().FirstOrDefaultAsync();

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

        public async Task<TrHead> GetTrHeadByTrNumber(long Trnum)
        {
            try
            {
                TrHead? th = await _dbContext.TrHeads.Where(x => x.TrhNo == Trnum).AsNoTracking().FirstOrDefaultAsync();

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

        public async Task<List<TrHead>> GetTrHeads()
        {
            try
            {
                return await _dbContext.TrHeads.OrderByDescending(y => y.TrhNo).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> UpdateTrHead(TrHead updatedTrHead)
        {
            try
            {
                TrHead? th1 = await _dbContext.TrHeads.Where(x => x.TrhId == updatedTrHead.TrhId).FirstOrDefaultAsync();
                if (th1 != null)
                {
                    th1.TrhNo = updatedTrHead.TrhNo;
                    th1.TrhDispNo = updatedTrHead.TrhDispNo;
                    th1.TrhDate = updatedTrHead.TrhDate;
                    th1.TrhUser = updatedTrHead.TrhUser;
                    th1.TrhDateAltered = updatedTrHead.TrhDateAltered;
                    th1.TrhApproved = updatedTrHead.TrhApproved;
                    th1.TrhRemarks = updatedTrHead.TrhRemarks;
                    th1.TrhExcludeAlertAction = updatedTrHead.TrhExcludeAlertAction;
                    th1.TrhComp = updatedTrHead.TrhComp;
                    th1.TrhBranch = updatedTrHead.TrhBranch;
                    _dbContext.TrHeads.Update(th1);
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
