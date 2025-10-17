using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IStockTransService
    {
        public Task<List<StockTran>> GetStockTransByDate(DateTime vStDate, DateTime vEnDate);
        public Task UpdateMultipleTablesAsync(string vCname, string vListNo, string vBatchId, decimal vPurchPrice,decimal vNewPrice);
        public Task UpdateSmultipleTablesAsync(string vCname, string vListNo, string vBatchId, decimal vSalePrice, decimal vNewPrice);

    }
}
