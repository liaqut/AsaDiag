using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IBranchService
    {
        public Task<List<Branch>> GetBranches();
        public Task<Branch> GetBranch(int branchId);
        public Task<Branch> GetBranchAdmin(int adminId);
        public Task<Branch> CreateBranch(Branch newBranch);
        public Task<string> UpdateBranch(Branch updatedBranch);
        public Task<string> DeleteBranch(int branchId);
    }
}
