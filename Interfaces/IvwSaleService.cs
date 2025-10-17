using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IvwSaleService
    {
        public Task<List<VwSale>> GetvwSales();
        public Task<List<VwSale>> GetvwSales(string vCname, string vListNo, string vBatchId, decimal vSalePrice);
        public Task<List<VwSale>> GetvwSalesDate(DateTime vStDate, DateTime vEnDate);
        public Task<VwSale> GetvwSalesStock(long vStkStockId);
        public Task<List<VwSale>> GetvwSalesStockList(long vStkStockId);

    }
}
