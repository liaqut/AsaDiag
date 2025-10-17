using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface ICatMasterService
    {
        public Task<List<CategMaster>> GetCategMasters();
        public Task<CategMaster> GetCategMaster(int catid);
        public Task<List<CategMaster>> GetCategMasters(string grpno);
        public Task<CategMaster> CreateCategMaster(CategMaster newCategMaster);
        public Task<string> UpdateCategMaster(CategMaster updatedCategMaster);
        public Task<string> DeleteCategMaster(int catid);
        public Task<List<string>> GetDDCategMasters();

    }
}
