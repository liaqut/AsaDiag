using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface ISystemPagesService
    {
        public Task<List<SystemPage>> GetSystemPages(string vComp);
    }
}
