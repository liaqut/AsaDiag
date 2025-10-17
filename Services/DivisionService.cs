using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace DigiEquipSys.Services
{
    public class DivisionService : IDivisionService
    {
        private readonly BASS_DBContext _dbContext = new();
        public DivisionService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Division> CreateDivision(Division newDivision)
        {
            try
            {
                var result = await this._dbContext.Divisions.AddAsync(newDivision);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteDivision(int divisionId)
        {
            try
            {
                Division? division = await _dbContext.Divisions.FindAsync(divisionId);

                if (division != null)
                {
                    _dbContext.Divisions.Remove(division);
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

        public async Task<Division> GetDivision(int divisionId)
        {
            try
            {
                Division? division = await _dbContext.Divisions.FindAsync(divisionId);

                if (division != null)
                {
                    return division;
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

        public async Task<List<Division>> GetDivisions()
        {
            try
            {
                return await _dbContext.Divisions.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Division>> GetDivisions(string branchCode)
        {
            try
            {
                return await _dbContext.Divisions.OrderBy(m => m.LocDesc).Where(x => x.LocBranchCode == branchCode).ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> UpdateDivision(Division updatedDivision)
        {
            try
            {
                Division? division1 = await _dbContext.Divisions.Where(x => x.LocId == updatedDivision.LocId).FirstOrDefaultAsync();
                if (division1 != null)
                {
                    division1.LocCode = updatedDivision.LocCode;
                    division1.LocBranchCode = updatedDivision.LocBranchCode;
                    division1.LocDesc = updatedDivision.LocDesc;
                    division1.LocAddress = updatedDivision.LocAddress;
                    division1.LocCity = updatedDivision.LocCity;
                    division1.LocState = updatedDivision.LocState;
                    _dbContext.Divisions.Update(division1);
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
