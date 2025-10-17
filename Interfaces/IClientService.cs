using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IClientService
    {
        public Task<List<ClientMaster>> GetClients();
        public Task<ClientMaster> GetClient(int clientId);
        public Task<ClientMaster> AddClient(ClientMaster newClient);
        public Task<string> UpdateClient(ClientMaster updatedClient);
        public Task<string> DeleteClient(int clientId);
    }
}
