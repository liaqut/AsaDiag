using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class vwTransferService : IvwTransferService
    {
        readonly BASS_DBContext _dbContext = new();

        public vwTransferService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<VwTransfer>> GetvwTransfers()
        {
            try
            {
                return await _dbContext.VwTransfers.Where(x => x.TrhApproved == true).ToListAsync();
            }
            catch
            {
                throw;
            }
        }
        public async Task<VwTransfer> GetvwTransferStock(long vStkStockId)
        {
            try
            {
                VwTransfer? rd = await _dbContext.VwTransfers.OrderByDescending(t => t.TrhDate).Where(x => x.TrdStockStkId == vStkStockId).AsNoTracking().FirstOrDefaultAsync();

                if (rd != null)
                {
                    return rd;
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

        public async Task<List<VwTransfer>> GetvwTransfersStockList(long vStkStockId)
        {
            try
            {
                return await _dbContext.VwTransfers.OrderByDescending(t => t.TrhDate).Where(x => x.TrdStockStkId == vStkStockId).ToListAsync();
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<VwTransfer>> GetvwTransfersDate(DateTime vStDate, DateTime vEnDate)
        {
            try
            {
				var start = vStDate.Date;
				var endExclusive = vEnDate.Date;
				return await _dbContext.VwTransfers.Where(x => x.TrhDate >= start && x.TrhDate < endExclusive && x.TrhApproved == true).OrderBy(z => z.TrhDate).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<VwTransfer>> GetvwTransfers(string vCname, string vListNo, string vBatchId, decimal vPurchPrice)
        {
            try
            {
                return await _dbContext.VwTransfers
                    .Where(x =>
                        x.TrhApproved == true &&
                        x.TrdListNo == vListNo &&
                        Convert.ToString(x.TrdBatchId) == vBatchId &&
                        (
                            (x.ClientFrom == vCname && x.TrdClientCodeFromUp == vPurchPrice) ||
                            (x.ClientTo == vCname && x.TrdClientCodeToUp == vPurchPrice)
                        )
                    )
                    .ToListAsync();
            }
            catch
            {
                throw;
            }
        }
        public async Task UpdateAction()
        {
            await _dbContext.Database.ExecuteSqlRawAsync("EXEC updAction");
        }

    }
}
