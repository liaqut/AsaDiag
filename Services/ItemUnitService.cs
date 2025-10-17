using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class ItemUnitService : IItemUnitService 
    {
        readonly BASS_DBContext _dbContext = new();
        public ItemUnitService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ItemUnit> CreateItemUnit(ItemUnit newItemUnit)
        {
            try
            {
                var result = await this._dbContext.ItemUnits.AddAsync(newItemUnit);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteItemUnit(int unitid)
        {
            try
            {
                ItemUnit? iu = _dbContext.ItemUnits.Find(unitid);

                if (iu != null)
                {
                    _dbContext.ItemUnits.Remove(iu);
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

        public async Task<ItemUnit> GetItemUnit(int unitid)
        {
            try
            {
                ItemUnit? iu = await _dbContext.ItemUnits.Where(x => x.ItemUnitId == unitid).FirstOrDefaultAsync();

                if (iu != null)
                {
                    return iu;
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

        public async Task<List<ItemUnit>> GetItemUnits()
        {
            try
            {
                return await _dbContext.ItemUnits.ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> UpdateItemUnit(ItemUnit updatedItemUnit)
        {
            try
            {
                ItemUnit? un1 = await _dbContext.ItemUnits.Where(x => x.ItemUnitId == updatedItemUnit.ItemUnitId).FirstOrDefaultAsync();
                if (un1 != null)
                {
                    un1.ItemUnitDesc = updatedItemUnit.ItemUnitDesc;
                    _dbContext.ItemUnits.Update(un1);
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


    }
}
