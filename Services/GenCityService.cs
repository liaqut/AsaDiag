using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class GenCityService : IGenCityService

    {
        readonly BASS_DBContext _dbContext = new();

        public GenCityService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        //To Get all user details   
        public async Task<List<GenCity>> GetGenCityDetails()
        {
            try
            {
                return await _dbContext.GenCities.OrderBy(x=>x.CityName).ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<GenCity>> GetCities(string cntrycode)
        {
            try
            {
                return await _dbContext.GenCities.OrderBy(m=>m.CityName).Where(x => x.CityCountryCode==cntrycode).ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<GenCity> AddGenCity(GenCity city)
        {
            try
            {
                var result = await this._dbContext.GenCities.AddAsync(city);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }

        }

        //To Update the records of a particluar user    
        public async Task<string> UpdateGenCityDetails(GenCity city)
        {
            try
            {
                GenCity? city1 = await _dbContext.GenCities.Where(x => x.CityId == city.CityId).FirstOrDefaultAsync();
                if (city1 != null)
                {
                    city1.CityCode = city.CityCode;
                    city1.CityName = city.CityName;
                    city1.CityCountryCode = city.CityCountryCode;
                    _dbContext.GenCities.Update(city1);
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
        public async Task<GenCity> GetGenCityData(int id)
        {
            try
            {
                GenCity? city = await _dbContext.GenCities.FindAsync(id);

                if (city != null)
                {
                    return city;
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
        public async Task<string> DeleteGenCity(int id)
        {
            try
            {
                GenCity? city = _dbContext.GenCities.Find(id);

                if (city != null)
                {
                    _dbContext.GenCities.Remove(city);
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
