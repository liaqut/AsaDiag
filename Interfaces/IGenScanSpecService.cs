using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface IGenScanSpecService
    {
        public Task<List<GenScanSpec>> GetGenScanSpecs();
        public Task<GenScanSpec> GetGenScanSpec(long genId);
        public Task<GenScanSpec> CreateGenScanSpec(GenScanSpec newGenScanSpec);
        public Task<string> UpdateGenScanSpec(GenScanSpec updatedGenScanSpec);
        public Task<string> DeleteGenScanSpec(long genId);
    }
}
