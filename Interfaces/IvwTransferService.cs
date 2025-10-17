using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IvwTransferService
    {
        public Task<List<VwTransfer>> GetvwTransfers();
        public Task<List<VwTransfer>> GetvwTransfers(string vCname, string vListNo, string vBatchId, decimal vPurchPrice);
        public Task<VwTransfer> GetvwTransferStock(long vStkStockId);
        public Task<List<VwTransfer>> GetvwTransfersDate(DateTime vStDate, DateTime vEnDate);
        public Task<List<VwTransfer>> GetvwTransfersStockList(long vStkStockId);
        public Task UpdateAction();

    }
}
