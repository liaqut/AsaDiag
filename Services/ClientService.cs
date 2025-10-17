using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class ClientService : IClientService
    {
        private readonly BASS_DBContext _dbContext = new();

        public ClientService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;

        }
        public async Task<ClientMaster> AddClient(ClientMaster newClient)
        {
            try
            {
                var result = await this._dbContext.ClientMasters.AddAsync(newClient);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteClient(int clientId)
        {
            try
            {
                ClientMaster? client = await _dbContext.ClientMasters.FindAsync(clientId);

                if (client != null)
                {
                    _dbContext.ClientMasters.Remove(client);
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

        public async Task<ClientMaster> GetClient(int clientId)
        {
            try
            {
                ClientMaster? client = await _dbContext.ClientMasters.FindAsync(clientId);

                if (client != null)
                {
                    return client;
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

        public async Task<List<ClientMaster>> GetClients()
        {
            try
            {
                return await _dbContext.ClientMasters.OrderBy(x=>x.ClientName).ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> UpdateClient(ClientMaster updatedClient)
        {
            try
            {
                ClientMaster? client1 = await this._dbContext.ClientMasters.Where(x => x.ClientId == updatedClient.ClientId).FirstOrDefaultAsync();
                if (client1 != null)
                {
                    client1.ClientCode = updatedClient.ClientCode;
                    client1.ClientName = updatedClient.ClientName;
                    client1.ClientVendCode = updatedClient.ClientVendCode;
                    client1.ClientContactPerson = updatedClient.ClientContactPerson;
                    client1.ClientAddr = updatedClient.ClientAddr;
                    client1.ClientTel = updatedClient.ClientTel;
                    client1.ClientEmail = updatedClient.ClientEmail;
                    client1.ClientUrl = updatedClient.ClientUrl;
                    client1.ClientCrNumber = updatedClient.ClientCrNumber;
                    client1.ClientRemarks = updatedClient.ClientRemarks;
                    client1.ClientZohoClientId=updatedClient.ClientZohoClientId;
                    this._dbContext.ClientMasters.Update(client1);
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
