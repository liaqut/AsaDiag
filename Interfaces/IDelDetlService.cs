using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IDelDetlService
    {
        public Task<List<DelDetl>> GetDelDetls();
        public Task<List<DelDetl>> GetDelDetlsByDelHeadId(long Delid);
        public Task<List<DelDetl>> GetDelDetlsByDelNumber(long Delhno);
        public Task<DelDetl> GetDelDetl(long Deldetlid);
        public Task<DelDetl> CreateDelDetl(DelDetl newDelDetl);
        public Task<string> UpdateDelDetl(DelDetl updatedDelDetl);
        public Task<string> DeleteDelDetl(long Deldetlid);
        public Task<string> DeleteDelDetlbyDelHead(long delheadId);

    }
}
