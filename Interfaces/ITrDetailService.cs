using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface ITrDetailService
    {
        public Task<List<TrDetail>> GetTrDetls();
        public Task<List<TrDetail>> GetTrDetls(DateTime vStDate, DateTime vEnDate);
		public Task<List<TrDetail>> GetTrDetlsByTrHeadId(long Trhid);
        public Task<TrDetail> GetTrDetl(long Trdetlid);
        public Task<TrDetail> CreateTrDetl(TrDetail newTrDetl);
        public Task<string> UpdateTrDetl(TrDetail updatedTrDetl);
        public Task<string> DeleteTrDetl(long Trdetlid);

		public Task UpdateTrdRev();

	}
}
