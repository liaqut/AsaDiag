using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IStockService
    {
        public Task<List<Stock>> GetAllStocks();
        public Task<List<Stock>> GetAllStocksNonZero();
        public Task<List<Stock>> GetStocks(string whcode);
        public Task<List<Stock>> GetStocks(long itemid);
        public Task<List<Stock>> GetStocksByItemCode(string itemcode);
        public Task<Stock> GetStock(long stkid);
        public Task<Stock?> GetStockDescending(string vList, string vLot, DateTime vDate, string vClcode);
        public Task<List<Stock?>> GetStockList(string vList, string vLot, DateTime vDate, string vClcode);
        public Task<Stock?> GetStock(string vList, string vLot, DateTime vDate,string vClcode);
        public Task<Stock?> GetStockBatch(string vList, string vLot, DateTime vDate, string vClcode, int vBatchId);

        public Task<VwBalQty?> GetStockBalance(string vList, string vLot, DateTime vDate, string vClcode);
        public Task<List<VwItemBal?>> GetVendDates(long vitemId);

        public Task<Stock?> GetStock(long vItemId, string vWhcode);
        public Task<Stock> CreateStock(Stock newStock);
        public Task<string> UpdateStock(Stock updatedStock);
        public Task<string> DeleteStock(long stkid);
        //public Task<List<Stock>> GetStocksByLocAndItem(string loc, string item); 
        public Task<List<StockForJournal>> GetStockForJournal();
        public Task<List<VwStockForPriceUpd>> GetStockForPriceUpd();
        public Task<List<VwStockForSpriceUpd>> GetStockForSpriceUpd();
        public Task<List<StockForJournal>> GetStockForJournal(int clId);
        public Task UpdateStockExpiryStatus();

        public Task ExecuteWithTransactionAsync(Func<Task> operation);
    }
}
