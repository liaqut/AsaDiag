using DigiEquipSys.Models;

namespace DigiEquipSys.Interfaces
{
    public interface ISupplierService
    {
        public Task<List<SupplierMaster>> GetSuppliers();
        public Task<SupplierMaster> GetSupplier(int supplierId);
        public Task<SupplierMaster> GetSupplierbyDetailCode(string detcode,string vRacc);
        public Task<SupplierMaster> AddSupplier(SupplierMaster newSupplier);
        public Task<string> UpdateSupplier(SupplierMaster updatedSupplier);
        public Task<string> DeleteSupplier(int supplierId);
    }
}
