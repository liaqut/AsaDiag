using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class ItemMasterService : IItemMasterService
    {
        readonly BASS_DBContext _dbContext = new();

        public ItemMasterService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ItemMaster> CreateItemMaster(ItemMaster newItemMaster)
        {
            try
            {
                var result = await this._dbContext.ItemMasters.AddAsync(newItemMaster);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteItemMaster(long itemid)
        {
            try
            {
                ItemMaster? im = _dbContext.ItemMasters.Find(itemid);

                if (im != null)
                {
                    _dbContext.ItemMasters.Remove(im);
                    await _dbContext.SaveChangesAsync();
                    return "Success";
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            catch (Exception ex)
            {
                string GetError = ex.Message;
                return "ERROR";
            }
        }

        public async Task<ItemMaster> GetItemMaster(long itemid)
        {
            try
            {
                ItemMaster? im = await _dbContext.ItemMasters.Where(x => x.ItemId == itemid).AsNoTracking().FirstOrDefaultAsync();

                if (im != null)
                {
                    return im;
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            catch
            {
                throw;
            }
        }

        public  async Task<ItemMaster> GetItemMasterByScanCode(string itcode)
        {
            try
            {
                ItemMaster? im1 = await _dbContext.ItemMasters.AsNoTracking().FirstOrDefaultAsync(x => x.ItemScanCode == itcode);

                if (im1 != null)
                {
                    return im1 ;
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<ItemMaster>> GetItemMasters()
        {
            try
            {
                return await _dbContext.ItemMasters.AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<ItemMaster>> GetItemMastersTest()
        {
            try
            {
                return await _dbContext.ItemMasters.Where(x=>x.ItemClientCode=="0001").AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<ItemMaster>> GetItemMastersDistinct()
        {
            try
            {
               return await _dbContext.ItemMasters.OrderBy(x => x.ItemDesc).GroupBy(p => p.ItemDesc).Select(g => g.First()).AsNoTracking().ToListAsync();

            }
            catch
            {
                throw;
            }
        }

        public async Task<List<ItemMaster>> GetItemMastersDistinct(string clcode)
        {
            try
            {
                return await _dbContext.ItemMasters.Where(x=>x.ItemClientCode==clcode).OrderBy(x => x.ItemDesc).GroupBy(p => p.ItemDesc).Select(g => g.First()).AsNoTracking().ToListAsync();

            }
            catch
            {
                throw;
            }
        }
        public async Task<List<ItemGroup>> GetItemGroups()
        {
            try
            {
                var imlist = await (from i in _dbContext.ItemMasters
                              join j in _dbContext.GroupMasters on i.ItemGrpCode equals j.GrpNo
                              select new ItemGroup
                              {
                                  ItemId=i.ItemId,
                                  GrpDesc=j.GrpDesc
                              }).ToListAsync();
                return imlist;
            }
            catch
            {
                throw;
            }
        }


        public async Task<List<ItemCat>> GetItemCats()
        {
            try
            {
                var catlist = await (from i in _dbContext.ItemMasters
                                    join j in _dbContext.CategMasters on i.ItemCatCode equals j.CatNo
                                    select new ItemCat
                                    {
                                        ItemId = i.ItemId,
                                        CatDesc = j.CatDesc
                                    }).ToListAsync();
                return catlist;
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> UpdateItemMaster(ItemMaster updatedItemMaster)
        {
            try
            {
                ItemMaster? th1 =await _dbContext.ItemMasters.Where(x => x.ItemId == updatedItemMaster.ItemId).FirstOrDefaultAsync();
                if (th1 != null)
                {
                    th1.ItemGrpCode = updatedItemMaster.ItemGrpCode;
                    th1.ItemCatCode = updatedItemMaster.ItemCatCode;
                    th1.ItemScanCode = updatedItemMaster.ItemScanCode;
                    th1.ItemListNo= updatedItemMaster.ItemListNo;   
                    th1.ItemDesc = updatedItemMaster.ItemDesc;
                    th1.ItemUnit = updatedItemMaster.ItemUnit;
                    th1.ItemProdCode = updatedItemMaster.ItemProdCode;  
                    th1.ItemSuppCode= updatedItemMaster.ItemSuppCode;
                    th1.ItemClientCode = updatedItemMaster.ItemClientCode;
                    th1.ScanCodeLength = updatedItemMaster.ScanCodeLength;
                    th1.ItemCostPrice = updatedItemMaster.ItemCostPrice;
                    th1.ItemSellPrice = updatedItemMaster.ItemSellPrice;
                    th1.ItemGst = updatedItemMaster.ItemGst;
                    th1.ItemHsnCode = updatedItemMaster.ItemHsnCode;
                    th1.ItemListNoProd=updatedItemMaster.ItemListNoProd;
                    th1.ItemCostPricePrev=updatedItemMaster.ItemCostPricePrev;
                    th1.ItemSellPricePrev=updatedItemMaster.ItemSellPricePrev;
                    th1.ItemZohoItemId=updatedItemMaster.ItemZohoItemId;
                    _dbContext.ItemMasters.Update(th1);
                    await _dbContext.SaveChangesAsync();
                }
                return "Success";
            }
            catch (Exception ex)
            {
                string GetError = ex.Message;
                return "ERROR";
            }
        }

        public async Task<ItemMaster> GetItemMaster(string listno, string clcode)
        {
            try
            {
                ItemMaster? im1 = await _dbContext.ItemMasters.Where(x => x.ItemListNo == listno && x.ItemClientCode==clcode).AsNoTracking().FirstOrDefaultAsync();

                if (im1 != null)
                {
                    return im1;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<ItemMaster> GetItemMaster(string listno)
        {
            try
            {
                ItemMaster? im1 = await _dbContext.ItemMasters.Where(x => x.ItemListNo == listno ).AsNoTracking().FirstOrDefaultAsync();

                if (im1 != null)
                {
                    return im1;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
