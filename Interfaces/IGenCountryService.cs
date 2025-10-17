using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IGenCountryService
    {
        public Task<List<GenCountry>> GetGenCountryDetails();

        public Task<GenCountry> AddGenCountry(GenCountry country);

        public Task<string> UpdateGenCountryDetails(GenCountry country);

        public Task<GenCountry> GetGenCountryData(int id);

        public Task<string> DeleteGenCountry(int id);
    }
}
