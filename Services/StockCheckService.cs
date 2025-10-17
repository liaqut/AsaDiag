using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.Charts.Chart.Internal;

namespace DigiEquipSys.Services
{
    public class StockCheckService : IStockCheckService
    {
        private readonly BASS_DBContext _dbContext = new();

        public StockCheckService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;

        }

        public async Task<StockCheck> AddStockCheck(StockCheck newStockCheck)
        {
            try
            {
                var result = await this._dbContext.StockChecks.AddAsync(newStockCheck);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteStockCheck(long Id)
        {
            try
            {
                StockCheck? st = _dbContext.StockChecks.Find(Id);

                if (st != null)
                {
                    _dbContext.StockChecks.Remove(st);
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

        public async Task<string> DelStockChecks()
        {
            try
            {
               await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM StockCheck");
                return "Success";
            }
            catch (Exception ex)
            {
                string GetError = ex.Message;
                return "ERROR";
            }
        }

        public async Task<List<StockCheck>> GetStockChecks()
        {
            try
            {
                return await _dbContext.StockChecks.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> UpdateStockCheck(StockCheck updatedStockCheck)
        {
            try
            {
                StockCheck? stk1 = await this._dbContext.StockChecks.Where(x => x.Id == updatedStockCheck.Id).FirstOrDefaultAsync();
                if (stk1 != null)
                {
                    stk1.RdScanCode = updatedStockCheck.RdScanCode;
                    stk1.RdListNo = updatedStockCheck.RdListNo;
                    stk1.RdLotNo = updatedStockCheck.RdLotNo;
                    stk1.RdExpiryDate = updatedStockCheck.RdExpiryDate;
                    stk1.RdQty = updatedStockCheck.RdQty;
                    stk1.RdStkId = updatedStockCheck.RdStkId;
                    this._dbContext.StockChecks.Update(stk1);
                    await this._dbContext.SaveChangesAsync();
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
