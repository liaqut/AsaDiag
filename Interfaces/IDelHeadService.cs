using DigiEquipSys.Models;
using System.Numerics;
using static DigiEquipSys.Pages.Del_pg;

namespace DigiEquipSys.Interfaces
{
    public interface IDelHeadService
    {
        public Task<BigInteger> GetNextDelNoAsync();

        public Task<List<DelHead>> GetDelHeads();
        public Task<List<DelHeadResult>> GetDelHeadResults();

        public Task<List<DelHead>> GetDelHeads(string loc);
        public Task<DelHead> GetDelHead(long Delheadid);
        public Task<DelHead> GetDelHeadByDelNumber(long Delnum);
        public Task<DelHead> CreateDelHead(DelHead newDelHead);
        public Task<string> UpdateDelHead(DelHead updatedDelHead);
        public Task<string> DeleteDelHead(long Delheadid);
        //public Task<List<DelSale>> GetDelHeadSale();
        public Task<DelHead> GetDelHeadByDispNo(string dispnum);
    }
}
