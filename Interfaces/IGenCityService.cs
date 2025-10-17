using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IGenCityService
    {
        public Task<List<GenCity>> GetGenCityDetails();
        public Task<List<GenCity>> GetCities(string cntrycode);

        public Task<GenCity> AddGenCity(GenCity city);

        public Task<string> UpdateGenCityDetails(GenCity city);

        public Task<GenCity> GetGenCityData(int id);

        public Task<string> DeleteGenCity(int id);
    }
}
