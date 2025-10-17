using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class GenCountryService : IGenCountryService
    {
        readonly BASS_DBContext _dbContext = new();

        public GenCountryService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        //To Get all user details   
        public async Task<List<GenCountry>> GetGenCountryDetails()
        {
            try
            {
                return await _dbContext.GenCountries.ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        //To Add new user record     
        public async Task<GenCountry> AddGenCountry(GenCountry country)
        {
            try
            {
                var result = await this._dbContext.GenCountries.AddAsync(country);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //To Update the records of a particluar user    
        public async Task<string> UpdateGenCountryDetails(GenCountry country)
        {
            try
            {
                GenCountry? country1 = await _dbContext.GenCountries.Where(x => x.CountryId == country.CountryId).FirstOrDefaultAsync();
                if (country1 != null)
                {
                    country1.CountryCode = country.CountryCode;
                    country1.CountryName = country.CountryName;

                    _dbContext.GenCountries.Update(country1);
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

        //Get the details of a particular user    
        public async Task<GenCountry> GetGenCountryData(int id)
        {
            try
            {
                GenCountry? country = await _dbContext.GenCountries.FindAsync(id);

                if (country != null)
                {
                    return country;
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

        //To Delete the record of a particular user    
        public async Task<string> DeleteGenCountry(int id)
        {
            try
            {
                GenCountry? country = _dbContext.GenCountries.Find(id);

                if (country != null)
                {
                    _dbContext.GenCountries.Remove(country);
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

    }
}
