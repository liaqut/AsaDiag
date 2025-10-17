using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IvwReceiptService
    {
        public Task<List<VwReceipt>> GetvwReceipts();
        public Task<List<VwReceipt>> GetvwReceipts(string vCname, string vListNo, string vBatchId, decimal vPurchPrice);

        public Task<List<VwReceipt>> GetvwReceiptsDate(DateTime vStDate, DateTime vEnDate);

    }
}
