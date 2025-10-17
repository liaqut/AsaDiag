using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IRoleService
    {
        public Task<List<RoleInfo>> GetRoleDetails();

    }
}
