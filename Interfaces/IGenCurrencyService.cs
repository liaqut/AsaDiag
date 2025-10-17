using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IGenCurrencyService
    {
        public Task<List<GenCurrency>> GetCurrencies();
        public Task<GenCurrency> GetCurrency(short currencyid);
        public Task<GenCurrency> CreateCurrency(GenCurrency newCurrency);
        public Task<string> UpdateCurrency(GenCurrency updatedCurrency);
        public Task<string> DeleteCurrency(short currencyid);
    }
}
