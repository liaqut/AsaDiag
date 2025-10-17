using DigiEquipSys.Models;
using System.Numerics;

namespace DigiEquipSys.Interfaces
{
    public interface IPoHeadService
    {
        public Task<BigInteger> GetNextPohNoAsync();

        public Task<List<PoHead>> GetPoHeads();
        public Task<PoHead> GetPoHead(long poheadid);
        public Task<List<PoHead>> GetPoHeads(int suppId);
        public Task<List<PoHead>> GetPoHeads(string suppcode);
        public Task<PoHead> GetPoHeadByPoNumber(long ponum);
        public Task<PoHead> GetPoHeadByPoNumber(string ponum);

        public Task<PoHead> CreatePoHead(PoHead newPoHead);
        public Task<string> UpdatePoHead(PoHead updatedPoHead);
        public Task<string> DeletePoHead(long poheadid);
    }
}

