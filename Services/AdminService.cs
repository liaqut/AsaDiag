using DigiEquipSys.Models;
using DigiEquipSys.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class AdminService : IAdminService
    {
        private readonly BASS_DBContext _dbContext;

        public AdminService(BASS_DBContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<AdminInfo> CreateAdminInfo(AdminInfo newAdminInfo)
        {
            try
            {
                var result = await this._dbContext.AdminInfos.AddAsync(newAdminInfo);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<AdminInfo> GetAdminDetail(string user)
        {
            try
            {
                AdminInfo? adminInfo = await _dbContext.AdminInfos.Where(x => x.Email == user).FirstOrDefaultAsync();

                if (adminInfo != null)
                {
                    return adminInfo;
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

        public async Task<List<AdminInfo>> GetAdminDetails()
        {
            try
            {
                return await _dbContext.AdminInfos.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> UpdateAdminInfo(AdminInfo updatedAdminInfo)
        {
            try
            {
                AdminInfo? admininfo1 = await _dbContext.AdminInfos.Where(x => x.Id == updatedAdminInfo.Id).FirstOrDefaultAsync();
                if (admininfo1 != null)
                {
                    admininfo1.Name = updatedAdminInfo.Name;
                    admininfo1.Email = updatedAdminInfo.Email;
                    admininfo1.Password = updatedAdminInfo.Password;
                    admininfo1.UpdatedOn = updatedAdminInfo.UpdatedOn;
                    admininfo1.RoleId = updatedAdminInfo.RoleId;
                    admininfo1.LocId = updatedAdminInfo.LocId;
                    _dbContext.AdminInfos.Update(admininfo1);
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

        public async Task<string> DeleteAdminInfo(int userId)
        {
            try
            {
                AdminInfo? adminInfo = await _dbContext.AdminInfos.FindAsync(userId);

                if (adminInfo != null)
                {
                    _dbContext.AdminInfos.Remove(adminInfo);
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
