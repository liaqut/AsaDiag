using DigiEquipSys.Models;
using System.Numerics;

namespace DigiEquipSys.Interfaces
{
    public interface IRcptHeadService
    {
        public Task<BigInteger> GetNextRhNoAsync();

        public Task<List<RcptHead>> GetRcptHeads();
        public Task<List<RcptHead>> GetRcptHeads(string vWhcode);
        public Task<RcptHead> GetRcptHeadbyRcptNo(long rcptno);
        public Task<RcptHead> AddRcptHead(RcptHead rcpt);

        public Task<string> UpdateRcptHead(RcptHead rcpt);

        public Task<RcptHead> GetRcptHead(long id);

        public Task<string> DeleteRcptHead(long id);
    }
}

