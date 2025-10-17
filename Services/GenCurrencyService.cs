using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class GenCurrencyService : IGenCurrencyService
    {
        readonly BASS_DBContext _dbContext = new();

        public GenCurrencyService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<GenCurrency> CreateCurrency(GenCurrency newCurrency)
        {
            try
            {
                var result = await this._dbContext.GenCurrencies.AddAsync(newCurrency);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteCurrency(short currencyid)
        {
            try
            {
                GenCurrency cu = _dbContext.GenCurrencies.Find(currencyid);

                if (cu != null)
                {
                    _dbContext.GenCurrencies.Remove(cu);
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

        public async Task<List<GenCurrency>> GetCurrencies()
        {
            try
            {
                return await _dbContext.GenCurrencies.ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<GenCurrency> GetCurrency(short currencyid)
        {
            try
            {
                GenCurrency cu = await _dbContext.GenCurrencies.Where(x => x.CurrId == currencyid).FirstOrDefaultAsync();

                if (cu != null)
                {
                    return cu;
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

        public async Task<string> UpdateCurrency(GenCurrency updatedCurrency)
        {
            try
            {
                GenCurrency td1 = await _dbContext.GenCurrencies.Where(x => x.CurrId == updatedCurrency.CurrId).FirstOrDefaultAsync();
                if (td1 != null)
                {
                    td1.CurrShortName = updatedCurrency.CurrShortName;
                    td1.CurrLongName = updatedCurrency.CurrLongName;
                }
                _dbContext.GenCurrencies.Update(td1);
                await _dbContext.SaveChangesAsync();
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
