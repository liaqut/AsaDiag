using DigiEquipSys.Models;
using System.Numerics;

namespace DigiEquipSys.Interfaces
{
    public interface ITrHeadService
    {
        public Task<BigInteger> GetNextTrNoAsync();
        public Task<List<TrHead>> GetTrHeads();
        public Task<TrHead> GetTrHead(long Trheadid);
        public Task<TrHead> GetTrHeadByTrNumber(long Trnum);
        public Task<TrHead> GetTrHeadByDispNo(string Trdispnum);
        public Task<TrHead> CreateTrHead(TrHead newTrHead);
        public Task<string> UpdateTrHead(TrHead updatedTrHead);
        public Task<string> DeleteTrHead(long Trheadid);

    }
}
