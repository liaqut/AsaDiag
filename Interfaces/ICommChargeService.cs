using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface ICommChargeService
    {
        public Task<List<CommCharge>> GetCommCharges();
        public Task<CommCharge> GetCommCharge(long commid);
        public Task<CommCharge> CreateCommCharge(CommCharge newCommCharge);
        public Task<string> UpdateCommCharge(CommCharge updatedCommCharge);
        public Task<string> DeleteCommCharge(long commid);
        public Task<List<CommCharge>> GetCommChargeDate(DateTime vStDate, DateTime vEnDate);
	}
}
