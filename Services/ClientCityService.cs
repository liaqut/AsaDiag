using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.Charts.Chart.Internal;

namespace DigiEquipSys.Services
{
    public class ClientCityService : IClientCityService
    {
        private readonly BASS_DBContext _dbContext = new();

        public ClientCityService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;

        }
        public async Task<ClientCity> AddClientCity(ClientCity newClientCity)
        {
            try
            {
                var result = await this._dbContext.ClientCities.AddAsync(newClientCity);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteClientCity(int clientcityId)
        {
            try
            {
                ClientCity? clientcity = await _dbContext.ClientCities.FindAsync(clientcityId);

                if (clientcity != null)
                {
                    _dbContext.ClientCities.Remove(clientcity);
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

        public async Task<List<ClientCity>> GetClientCities()
        {
            try
            {
                return await  _dbContext.ClientCities.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
		}
        public async Task<List<ClientCity>> GetClientCities(int clientId)
        {
            try
            {
				return await _dbContext.ClientCities.Where(x => x.ClientId == clientId).ToListAsync();
			}
			catch (Exception)
            {
                throw;
            }
        }
        public async Task<ClientCity> GetClientCity(int clientcityId)
        {
            try
            {
                ClientCity? clientcity = await _dbContext.ClientCities.FindAsync(clientcityId);

                if (clientcity != null)
                {
                    return clientcity;
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

        public async Task<string> UpdateClientCity(ClientCity updatedClientCity)
        {
            try
            {
                ClientCity? clientcity1 = await this._dbContext.ClientCities.Where(x => x.ClientCityId == updatedClientCity.ClientCityId).FirstOrDefaultAsync();
                if (clientcity1 != null)
                {
                    clientcity1.ClientId = updatedClientCity.ClientId;
                    clientcity1.CityId = updatedClientCity.CityId;
                    this._dbContext.ClientCities.Update(clientcity1);
                    await this._dbContext.SaveChangesAsync();
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
