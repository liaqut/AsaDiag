using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface ISDelDetlService
    {
        public Task<List<SdelDetl>> GetSDelDetls();
        public Task<List<SdelDetl>> GetSDelDetlsBySdelHeadId(long SdelHeadid);
        public Task<SdelDetl> GetSDelDetl(long sDeldetlid);
        public Task<SdelDetl> CreateSDelDetl(SdelDetl newSDelDetl);
        public Task<string> UpdateSDelDetl(SdelDetl updateSDelDetl);
        public Task<string> DeleteSDelDetl(long sDeldetlid);
        public Task DeleteBySdelHeadId(long ids);
    }
}
