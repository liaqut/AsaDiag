using DigiEquipSys.Models;
namespace DigiEquipSys.Interfaces
{
    public interface IAdminService
    {
        public Task<List<AdminInfo>> GetAdminDetails();
        public Task<AdminInfo> GetAdminDetail(string user);
        public Task<AdminInfo> CreateAdminInfo(AdminInfo newAdminInfo);
        public Task<string> UpdateAdminInfo(AdminInfo updatedAdminInfo);
        public Task<string> DeleteAdminInfo(int userId);


    }
}
