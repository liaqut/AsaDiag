using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces

{
    public interface IMenuInfo
    {
        public Task<List<MenuInfo>> GetMenuDetails();
    }
}