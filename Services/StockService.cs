using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.Data;

namespace DigiEquipSys.Services
{
    public class StockService : IStockService
    {
        readonly BASS_DBContext _dbContext = new();

        public StockService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ExecuteWithTransactionAsync(Func<Task> operation)
        { var strategy = _dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () => 
            { using (var transaction = await _dbContext.Database.BeginTransactionAsync()) 
                { try
                    {
                        await operation().ConfigureAwait(false); 
                        await transaction.CommitAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        throw;
                    } 
                } 
            }); 
        }

        public async Task<Stock> CreateStock(Stock newStock)
        {
            try
            {
                var result = await this._dbContext.Stocks.AddAsync(newStock);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteStock(long stkid)
        {
            try
            {
                Stock? st = _dbContext.Stocks.Find(stkid);

                if (st != null)
                {
                    _dbContext.Stocks.Remove(st);
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

        public async Task<Stock> GetStock(long stkid)
        {
            try
            {
                Stock? st = await _dbContext.Stocks.FindAsync(stkid);

                if (st != null)
                {
                    return st;
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

        public async Task<Stock?> GetStock(long vItemId,string vClcode)
        {
            try
            {
                return await _dbContext.Stocks.Where(x => x.ItemId==vItemId && x.ItemClientCode== vClcode).AsNoTracking().FirstOrDefaultAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<Stock?> GetStockDescending(string vList, string vLot, DateTime vDate,string vClcode)
        {
            try
            {
                return await _dbContext.Stocks.Where(x => x.ItemListNo==vList && x.ItemLotNo==vLot && x.ItemExpiryDate==vDate && x.ItemClientCode==vClcode).OrderByDescending(m=>m.ItemBatchId).AsNoTracking().FirstOrDefaultAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Stock?>> GetStockList(string vList, string vLot, DateTime vDate, string vClcode)
        {
            try
            {
                return await _dbContext.Stocks.Where(x => x.ItemListNo == vList && x.ItemLotNo == vLot && x.ItemExpiryDate == vDate && x.ItemClientCode == vClcode).OrderBy(m => m.ItemBatchId).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<Stock?> GetStockBatch(string vList, string vLot, DateTime vDate, string vClcode,int vBatchId)
        {
            try
            {
                return await _dbContext.Stocks.Where(x => x.ItemListNo == vList && x.ItemLotNo == vLot && x.ItemExpiryDate == vDate && x.ItemClientCode == vClcode && x.ItemBatchId==vBatchId).AsNoTracking().FirstOrDefaultAsync();
            }
            catch
            {
                throw;
            }
        }
        public async Task<Stock?> GetStock(string vList, string vLot, DateTime vDate, string vClcode)
        {
            try
            {
                return await _dbContext.Stocks.Where(x => x.ItemListNo == vList && x.ItemLotNo == vLot && x.ItemExpiryDate == vDate && x.ItemClientCode == vClcode).AsNoTracking().FirstOrDefaultAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<VwBalQty?> GetStockBalance(string vList, string vLot, DateTime vDate, string vClcode)
        {
            try
            {
                return await _dbContext.VwBalQties.Where(x => x.ItemListNo == vList && x.ItemLotNo == vLot && x.ItemExpiryDate == vDate && x.ItemClientCode == vClcode).AsNoTracking().FirstOrDefaultAsync();
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<VwItemBal?>> GetVendDates(long itemid)
        {
            try
            {
                return await _dbContext.VwItemBals.Where(x => x.StkId == itemid).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<Stock>> GetStocks(string whcode)
        {
            try
            {
                return await _dbContext.Stocks.AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Stock>> GetStocks(long itemid)
        {
            try
            {
                return await _dbContext.Stocks.Where(x => x.ItemId == itemid).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Stock>> GetStocksByItemCode(string itemcode)
        {
            try
            {
                var qry = (from x in _dbContext.Stocks join y in _dbContext.ItemMasters on x.ItemId equals y.ItemId where (y.ItemListNo.Contains(itemcode) || y.ItemDesc.Contains(itemcode)) select x).AsNoTracking().ToListAsync();
                return await qry;
            }
            catch
            {
                throw;
            }
        }


        public async Task<List<Stock>> GetAllStocks()
        {
            try
            {
                return await _dbContext.Stocks.AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Stock>> GetAllStocksNonZero()
        {
            try
            {
                return await _dbContext.Stocks.Where(x=>(((x.ItemOpQty ?? 0) + (x.ItemPurQty ?? 0) + (x.ItemTrInQty ?? 0)) - ((x.ItemTrOutQty ?? 0) + (x.ItemDelQty ?? 0))) != 0).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<StockForJournal>> GetStockForJournal()
        {
            try
            {
                return await _dbContext.StockForJournals.AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<VwStockForPriceUpd>> GetStockForPriceUpd()
        {
            try
            {
                return await _dbContext.VwStockForPriceUpds.AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<VwStockForSpriceUpd>> GetStockForSpriceUpd()
        {
            try
            {
                return await _dbContext.VwStockForSpriceUpds.AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<StockForJournal>> GetStockForJournal(int clId)
        {
            try
            {
                var vStkJournal = await (from x in _dbContext.StockForJournals join y in _dbContext.ClientMasters on x.ClientName equals y.ClientName where y.ClientId == clId select x).AsNoTracking().ToListAsync();
                return vStkJournal;
            }
            catch
            {
                throw;
            }
        }
        public async Task<string> UpdateStock(Stock updatedStock)
        {
            try
            {
                Stock? stock1 = await _dbContext.Stocks.Where(x => x.StkId == updatedStock.StkId).FirstOrDefaultAsync();
                if (stock1 != null)
                {
                    stock1.ItemId = updatedStock.ItemId;
                    stock1.ItemScanCode = updatedStock.ItemScanCode;
                    stock1.ItemListNo = updatedStock.ItemListNo;
                    stock1.ItemLotNo= updatedStock.ItemLotNo;
                    stock1.ItemExpiryDate = updatedStock.ItemExpiryDate;
                    stock1.ItemClientCode = updatedStock.ItemClientCode;
                    stock1.ItemUp = updatedStock.ItemUp;
                    stock1.ItemOpQty = updatedStock.ItemOpQty;
                    stock1.ItemSp = updatedStock.ItemSp;
                    stock1.ItemPurQty = updatedStock.ItemPurQty;
                    stock1.ItemPurAmt = updatedStock.ItemPurAmt;
                    stock1.ItemDelQty = updatedStock.ItemDelQty;
                    stock1.ItemDelAmt = updatedStock.ItemDelAmt;
                    stock1.ItemTrInQty = updatedStock.ItemTrInQty;
                    stock1.ItemTrInAmt= updatedStock.ItemTrInAmt;
                    stock1.ItemTrOutQty = updatedStock.ItemTrOutQty;
                    stock1.ItemTrOutAmt= updatedStock.ItemTrOutAmt;
                    stock1.ItemStkIdDesc = updatedStock.ItemStkIdDesc;
                    stock1.ItemStkIdGrp = updatedStock.ItemStkIdGrp;
                    stock1.ItemStkIdUnit = updatedStock.ItemStkIdUnit;
                    stock1.ItemStkIdCat = updatedStock.ItemStkIdCat;
                    stock1.ItemSuppCode = updatedStock.ItemSuppCode;
                    stock1.ItemExpStat=updatedStock.ItemExpStat;
                    stock1.ItemCp=updatedStock.ItemCp;
                    stock1.ItemBatchId= updatedStock.ItemBatchId;
                    _dbContext.Stocks.Update(stock1);
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

        public async Task UpdateStockExpiryStatus() 
        {
            await _dbContext.Database.ExecuteSqlRawAsync("EXEC updExp");
        }
    }

    //public async Task<List<Stock>> GetStocksByLocAndItem(string loc, string item)
    //{
    //    try
    //    {
    //        var qry = (from x in _dbContext.Stocks join y in _dbContext.ItemMasters on x.ItemId equals y.ItemId where (y.ItemCode.Contains(item) || y.ItemDesc.Contains(item))  select x).ToListAsync();
    //        return await qry;
    //    }
    //    catch
    //    {
    //        throw;
    //    }
    //}
}
