using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IPoDetailService
    {
        public Task<List<PoDetail>> GetPoDetails();
        public Task<PoDetail> GetPoDetail(long podetailid);
        public Task<List<PoDetail>> GetPoDetails(long pohid);
        public Task<List<VwPurchaseOrder>> GetvwPoByDate(DateTime vStDate, DateTime vEnDate);
        public Task<List<VwPurchaseOrder>> GetvwPendingPo(DateTime vStDate, DateTime vEnDate);
        public Task<List<VwPurchaseOrder>> GetvwExcessPo(DateTime vStDate, DateTime vEnDate);

        public Task<PoDetail> GetPoDetail(long pono,long stkcode);
        public Task<PoDetail> GetPoDetail(long pono, string listno);
        public Task<PoDetail> CreatePoDetail(PoDetail newPoDetail);
        public Task<string> UpdatePoDetail(PoDetail updatedPoDetail);
        public Task<bool> DeletePoDetail(long podetailid);
    }
}

