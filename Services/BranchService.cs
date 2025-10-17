using DigiEquipSys.Models;
using DigiEquipSys.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class BranchService : IBranchService
    {
        private readonly BASS_DBContext _dbContext = new();
        public BranchService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Branch> CreateBranch(Branch newBranch)
        {
            try
            {
                var result = await this._dbContext.Branches.AddAsync(newBranch);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteBranch(int branchId)
        {
            try
            {
                Branch? branch = await _dbContext.Branches.FindAsync(branchId);

                if (branch != null)
                {
                    _dbContext.Branches.Remove(branch);
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

        public async Task<Branch> GetBranch(int branchId)
        {
            try
            {
                Branch? branch = await _dbContext.Branches.FindAsync(branchId);

                if (branch != null)
                {
                    return branch;
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

        public async Task<Branch> GetBranchAdmin(int adminId)
        {
            try
            {
                Branch? branch = await _dbContext.Branches.Where(q => q.BranchAdminId == adminId).FirstOrDefaultAsync();

                if (branch != null)
                {
                    return branch;
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

        public async Task<List<Branch>> GetBranches()
        {
            try
            {
                return await _dbContext.Branches.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> UpdateBranch(Branch updatedBranch)
        {

            try
            {
                Branch? branch1 = await _dbContext.Branches.Where(x => x.BranchId == updatedBranch.BranchId).FirstOrDefaultAsync();
                if (branch1 != null)
                {
                    branch1.BranchCode = updatedBranch.BranchCode;
                    branch1.BranchDesc = updatedBranch.BranchDesc;
                    branch1.BranchAddress = updatedBranch.BranchAddress;
                    branch1.BranchCity = updatedBranch.BranchCity;
                    branch1.BranchCountry = updatedBranch.BranchCountry;
                    branch1.BranchCrNo = updatedBranch.BranchCrNo;
                    branch1.BranchPhone = updatedBranch.BranchPhone;
                    branch1.BranchPoBox = updatedBranch.BranchPoBox;
                    branch1.BranchState = updatedBranch.BranchState;
                    branch1.BranchVatNo = updatedBranch.BranchVatNo;
                    branch1.SetupPassword = updatedBranch.SetupPassword;
                    _dbContext.Branches.Update(branch1);
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
