using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IItemMasterService
    {
        public Task<List<ItemMaster>> GetItemMasters();
        public Task<List<ItemMaster>> GetItemMastersTest();
        public Task<List<ItemMaster>> GetItemMastersDistinct();
        public Task<List<ItemMaster>> GetItemMastersDistinct(string clcode  );
        public Task<List<ItemGroup>> GetItemGroups();
        public Task<List<ItemCat>> GetItemCats();

        public Task<ItemMaster> GetItemMaster(long itemid);
        public Task<ItemMaster> GetItemMaster(string listno, string clcode);
        public Task<ItemMaster> GetItemMaster(string listno);
        public Task<ItemMaster> GetItemMasterByScanCode(string itcode);
        public Task<ItemMaster> CreateItemMaster(ItemMaster newItemMaster);
        public Task<string> UpdateItemMaster(ItemMaster updatedItemMaster);
        public Task<string> DeleteItemMaster(long itemid);
    }
}
