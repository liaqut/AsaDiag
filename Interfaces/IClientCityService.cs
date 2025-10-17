using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IClientCityService
    {
        public Task<List<ClientCity>> GetClientCities();
        public Task<List<ClientCity>> GetClientCities(int clientId);
        public Task<ClientCity> GetClientCity(int clientcityId);
        public Task<ClientCity> AddClientCity(ClientCity newClientCity);
        public Task<string> UpdateClientCity(ClientCity updatedClientCity);
        public Task<string> DeleteClientCity(int clientcityId);
    }
}
