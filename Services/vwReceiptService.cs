using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace DigiEquipSys.Services
{
    public class vwReceiptService : IvwReceiptService
    {
        readonly BASS_DBContext _dbContext = new();

        public vwReceiptService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<VwReceipt>> GetvwReceipts()
        {
            try
            {
                return await _dbContext.VwReceipts.Where(x=>x.RhApproved==true).ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<VwReceipt>> GetvwReceiptsDate(DateTime vStDate, DateTime vEnDate)
        {
            try
            {
				//DateTime vStDate1 = DateTime.ParseExact(vStDate.ToString(), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
				//DateTime vEnDate1 = DateTime.ParseExact(vEnDate.ToString(), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

				var start = vStDate.Date;
				var endExclusive = vEnDate.Date;
				return await _dbContext.VwReceipts.Where(x=>x.RdVendInvDate>= start && x.RdVendInvDate < endExclusive && x.RhApproved == true).OrderBy (z=>z.RdVendInvDate).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<VwReceipt>> GetvwReceipts(string vCname,string vListNo, string vBatchId, decimal vPurchPrice )
        {
            try
            {
                return await _dbContext.VwReceipts.Where(x => x.RhApproved == true && x.ClientName==vCname && x.RdListNo==vListNo && x.RdUp==vPurchPrice).ToListAsync();
            }
            catch
            {
                throw;
            }
        }


    }
}
