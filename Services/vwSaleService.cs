using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace DigiEquipSys.Services
{
    public class vwSaleService : IvwSaleService
    {
        readonly BASS_DBContext _dbContext = new();

        public vwSaleService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<VwSale>> GetvwSales()
        {
            try
            {
                return await _dbContext.VwSales.Where(x => x.DelApproved == true).ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<VwSale>> GetvwSales(string vCname, string vListNo, string vBatchId, decimal vSalePrice)
        {
            try
            {
                return await _dbContext.VwSales.Where(x => x.DelApproved == true && Convert.ToString(x.DelBatchId) == vBatchId && x.ClientName == vCname && x.DelListNo == vListNo && x.DelUprice == vSalePrice).ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<VwSale> GetvwSalesStock(long vStkStockId)
        {
            try
            {
                VwSale? rd = await _dbContext.VwSales.OrderByDescending(t=>t.DelDate).Where(x => x.DelStockStkId == vStkStockId).AsNoTracking().FirstOrDefaultAsync();

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
        public async Task<List<VwSale>> GetvwSalesStockList(long vStkStockId)
        {
            try
            {
                return await _dbContext.VwSales.OrderByDescending(t => t.DelDate).Where(x => x.DelStockStkId == vStkStockId).ToListAsync();
            }
            catch
            {
                throw;
            }
        }
        public async Task<List<VwSale>> GetvwSalesDate(DateTime vStDate, DateTime vEnDate)
        {
            try
            {
				var start = vStDate.Date;
				var endExclusive = vEnDate.Date;
				return await _dbContext.VwSales.Where(x => x.DelDate >= start && x.DelDate < endExclusive && x.DelApproved == true).OrderBy(z=>z.DelDate).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
