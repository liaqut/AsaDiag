using DigiEquipSys.Models;
using System.Numerics;

namespace DigiEquipSys.Interfaces
{
    public interface ISDelHeadService
    {
        public Task<BigInteger> GetNextSdelNoAsync();
        public Task<List<SdelHead>> GetSdelHeads();
        public Task<SdelHead> GetSdelHead(long SdelHeadid);
        public Task<SdelHead> GetSdelHeadBySDelNumber(long sDelnum);
        public Task<SdelHead> CreateSdelHead(SdelHead newSdelHead);
        public Task<string> UpdateSdelHead(SdelHead updateSdelHead);
        public Task<string> DeleteSdelHead(long SdelHeadid);
        public Task<SdelHead> GetSdelHeadByDelDispNo(string Deldispnum);
        public Task<SdelHead> GetSdelHeadAllByDelDispNo(string Deldispnum);
        public Task<SdelHead> GetSdelHead(DateTime StDate, DateTime EnDate);
    }
}