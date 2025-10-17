using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IStockCheckService
    {
        public Task<StockCheck> AddStockCheck(StockCheck newStockCheck);
        public Task<List<StockCheck>> GetStockChecks();
        public Task<string> DelStockChecks();
        public Task<string> UpdateStockCheck(StockCheck updatedStockCheck);
        public Task<string> DeleteStockCheck(long Id);
    }
}
