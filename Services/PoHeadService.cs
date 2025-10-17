using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace DigiEquipSys.Services
{
    public class PoHeadService : IPoHeadService

    {
        readonly BASS_DBContext _dbContext = new();

        public PoHeadService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<PoHead> CreatePoHead(PoHead newPoHead)
        {
            try
            {
                var result = await this._dbContext.PoHeads.AddAsync(newPoHead);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BigInteger> GetNextPohNoAsync()
        {
            var connection = _dbContext.Database.GetDbConnection();
            await connection.OpenAsync();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "GetNextPohNo";
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

        public async Task<string> DeletePoHead(long poheadid)
        {
            try
            {
                PoHead th = _dbContext.PoHeads.Find(poheadid);

                if (th != null)
                {
                    _dbContext.PoHeads.Remove(th);
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

        public async Task<PoHead> GetPoHead(long poheadid)
        {
            try
            {
                PoHead th = await _dbContext.PoHeads.Where(x => x.PohId == poheadid).FirstOrDefaultAsync();

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

        public async Task<PoHead> GetPoHeadByPoNumber(long ponum)
        {
            try
            {
                PoHead th = await _dbContext.PoHeads.Where(x => x.PohNo == ponum).FirstOrDefaultAsync();

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

        public async Task<PoHead> GetPoHeadByPoNumber(string ponum)
        {
            try
            {
                PoHead th = await _dbContext.PoHeads.Where(x => x.PohDispNo == ponum).FirstOrDefaultAsync();

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

        public async Task<List<PoHead>> GetPoHeads()
        {
            try
            {
                return await _dbContext.PoHeads.ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<PoHead>> GetPoHeads(int suppId)
        {
            try
            {
                return await _dbContext.PoHeads.Where(x=>x.PohVendId==suppId).ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<PoHead>> GetPoHeads(string suppcode)
        {
            try
            {
                return await (from px in _dbContext.PoHeads join s in _dbContext.SupplierMasters on px.PohVendId equals s.SuppId where s.SuppCode==suppcode select px).ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> UpdatePoHead(PoHead updatedPoHead)
        {
            try
            {
                PoHead th1 = await _dbContext.PoHeads.Where(x => x.PohId == updatedPoHead.PohId).FirstOrDefaultAsync();
                if (th1 != null)
                {
                    th1.PohNo = updatedPoHead.PohNo;
                    th1.PohDate = updatedPoHead.PohDate;
                    th1.PohCurr = updatedPoHead.PohCurr;
                    th1.PohVendId = updatedPoHead.PohVendId;
                    th1.PohCustId= updatedPoHead.PohCustId;
                    th1.PohVendRef = updatedPoHead.PohVendRef;
                    th1.PohConvRate = updatedPoHead.PohConvRate;
                    th1.PohApproved = updatedPoHead.PohApproved;
                    th1.PohRemarks = updatedPoHead.PohRemarks;
                    th1.PohUser=updatedPoHead.PohUser;
                    th1.PohDateAltered = updatedPoHead.PohDateAltered;
                    th1.PohConvRate=updatedPoHead.PohConvRate;
                    th1.PohDispNo= updatedPoHead.PohDispNo;
                    th1.PohComp= updatedPoHead.PohComp;
                    th1.PohBranch= updatedPoHead.PohBranch;
                }
                _dbContext.PoHeads.Update(th1);
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
