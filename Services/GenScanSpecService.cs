using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class GenScanSpecService : IGenScanSpecService
    {
        private readonly BASS_DBContext _dbContext = new();
        public GenScanSpecService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<GenScanSpec> CreateGenScanSpec(GenScanSpec newGenScanSpec)
        {
            try
            {
                var result = await this._dbContext.GenScanSpecs.AddAsync(newGenScanSpec);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteGenScanSpec(long genId)
        {
            try
            {
                GenScanSpec? scanspec = await _dbContext.GenScanSpecs.FindAsync(genId);

                if (scanspec != null)
                {
                    _dbContext.GenScanSpecs.Remove(scanspec);
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

        public async Task<GenScanSpec> GetGenScanSpec(long genId)
        {
            try
            {
                GenScanSpec? scan = await _dbContext.GenScanSpecs.FindAsync(genId);

                if (scan != null)
                {
                    return scan;
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

        public async Task<List<GenScanSpec>> GetGenScanSpecs()
        {
            try
            {
                return await _dbContext.GenScanSpecs.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> UpdateGenScanSpec(GenScanSpec updatedGenScanSpec)
        {
            try
            {
                GenScanSpec? scan1 = await _dbContext.GenScanSpecs.Where(x => x.GenId == updatedGenScanSpec.GenId).FirstOrDefaultAsync();
                if (scan1 != null)
                {
                    scan1.GenScanLength = updatedGenScanSpec.GenScanLength;
                    scan1.GenType = updatedGenScanSpec.GenType;
                    scan1.GenListLength = updatedGenScanSpec.GenListLength;
                    scan1.GenListStartFrom = updatedGenScanSpec.GenListStartFrom;
                    scan1.GenLotLength = updatedGenScanSpec.GenLotLength;
                    scan1.GenLotStartFrom = updatedGenScanSpec.GenLotStartFrom;
                    scan1.GenExpiryLength = updatedGenScanSpec.GenExpiryLength;
                    scan1.GenExpiryStartFrom = updatedGenScanSpec.GenExpiryStartFrom;
                    scan1.GenExpiryDir = updatedGenScanSpec.GenExpiryDir;

                    _dbContext.GenScanSpecs.Update(scan1);
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
