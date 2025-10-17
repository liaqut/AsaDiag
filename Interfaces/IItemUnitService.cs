using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IItemUnitService
    {
        public Task<List<ItemUnit>> GetItemUnits();
        public Task<ItemUnit> GetItemUnit(int unitid);
        public Task<ItemUnit> CreateItemUnit(ItemUnit newItemUnit);
        public Task<string> UpdateItemUnit(ItemUnit updatedItemUnit);
        public Task<string> DeleteItemUnit(int unitid);
    }
}
