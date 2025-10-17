using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface ISysPagesControlService
    {
        public Task<List<SysPagesControl>> GetSysPagesControls();
        public Task<SysPagesControl?> GetSysPagesControls(string mailId,int ctrlid);
        public Task<List<SysPagesControl>> GetSysPagesControls(string mailId,string vComp);
        public Task<SysPagesControl> GetSysPagesControl(int syspagesid);
        public Task<SysPagesControl> CreateSysPagesControl(SysPagesControl newSysPagesControl);
        public Task<string> UpdateSysPagesControl(SysPagesControl updatedSysPagesControl);
        public Task<string> DeleteSysPagesControl(int syspagesid);
        public Task<string> DeleteSysPagesControl(string email);
    }
}
