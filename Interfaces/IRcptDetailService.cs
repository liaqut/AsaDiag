using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IRcptDetailService
    {
        public Task<List<RcptDetail>> GetRcptDetails();
        public Task<RcptDetail> GetRcptDetail(long id);
        public Task<List<RcptDetail>> GetRcptDetails(long rcptserno);
        public Task<List<RcptDetail>> GetRcptDetails(string listno,string clcode, string pohno);
        public Task<RcptDetail> AddRcptDetail(RcptDetail newrcpt);
        public Task<string> UpdateRcptDetail(RcptDetail rcpt);
        public Task<bool> DeleteRcptDetail(long id);
        public Task<string> DeleteRcptDetailbyRcptHead(long rcptheadId);
        public Task<double> GetTotRcptQtyByPo(long pono, long stkid);
        public Task<(double,double)> GetTotRcptQtyByStkId(string whno, long stkid);

    }
}




