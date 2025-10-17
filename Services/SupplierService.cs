using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly BASS_DBContext _dbContext = new();

        public SupplierService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;

        }
        public async Task<SupplierMaster> AddSupplier(SupplierMaster newSupplier)
        {
            try
            {
                var result = await this._dbContext.SupplierMasters.AddAsync(newSupplier);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteSupplier(int supplierId)
        {
            try
            {
                SupplierMaster? supplier = _dbContext.SupplierMasters.Find(supplierId);

                if (supplier != null)
                {
                    _dbContext.SupplierMasters.Remove(supplier);
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

        public async Task<SupplierMaster> GetSupplier(int supplierId)
        {
            try
            {
                SupplierMaster? supplier = await _dbContext.SupplierMasters.FindAsync(supplierId);

                if (supplier != null)
                {
                    return supplier;
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

        public async Task<SupplierMaster> GetSupplierbyDetailCode(string detcode,string vRacc)
        {
            try
            {
                SupplierMaster? supplier = await _dbContext.SupplierMasters.Where(x => x.SuppCode == detcode).FirstOrDefaultAsync();

                if (supplier != null)
                {
                    return supplier;
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

        public async Task<List<SupplierMaster>> GetSuppliers()
        {
            try
            {
                return await _dbContext.SupplierMasters.ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> UpdateSupplier(SupplierMaster updatedSupplier)
        {
            try
            {
                SupplierMaster? th1 = await _dbContext.SupplierMasters.Where(x => x.SuppCode == updatedSupplier.SuppCode).FirstOrDefaultAsync();
                if (th1 != null)
                {
                    th1.SuppPhone = updatedSupplier.SuppPhone;
                    th1.SuppCode = updatedSupplier.SuppCode;
                    th1.SuppAddr = updatedSupplier.SuppAddr;
                    th1.SuppCity = updatedSupplier.SuppCity;
                    th1.SuppContPerson = updatedSupplier.SuppContPerson;
                    th1.SuppCountry = updatedSupplier.SuppCountry;
                    th1.SuppEMail = updatedSupplier.SuppEMail;
                    th1.SuppName = updatedSupplier.SuppName;
                    th1.SuppRemarks = updatedSupplier.SuppRemarks;
                    th1.SuppUrl = updatedSupplier.SuppUrl;
                    th1.SuppCrNo=updatedSupplier.SuppCrNo;  
                    th1.SuppZohoVendId= updatedSupplier.SuppZohoVendId;
                    _dbContext.SupplierMasters.Update(th1);
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
