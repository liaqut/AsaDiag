using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IDivisionService
    {
        public Task<List<Division>> GetDivisions();
        public Task<List<Division>> GetDivisions(string branchCode);

        public Task<Division> GetDivision(int divisionId);
        public Task<Division> CreateDivision(Division newDivision);
        public Task<string> UpdateDivision(Division updatedDivision);
        public Task<string> DeleteDivision(int divisionId);


    }
}
