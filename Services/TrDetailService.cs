using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;

namespace DigiEquipSys.Services
{
    public class TrDetailService : ITrDetailService
    {
        readonly BASS_DBContext _dbContext = new();

        public TrDetailService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TrDetail> CreateTrDetl(TrDetail newTrDetl)
        {
            try
            {
                var result = await this._dbContext.TrDetails.AddAsync(newTrDetl);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> DeleteTrDetl(long Trdetlid)
        {
            try
            {
               TrDetail? th = await _dbContext.TrDetails.FindAsync(Trdetlid);

                if (th != null)
                {
                    _dbContext.TrDetails.Remove(th);
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

        public async Task<TrDetail> GetTrDetl(long Trdetlid)
        {
            try
            {
                TrDetail? th = await _dbContext.TrDetails.Where(x => x.TrdId == Trdetlid).AsNoTracking().FirstOrDefaultAsync();

                if (th != null)
                {
                    return th;
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

		public async Task<List<TrDetail>> GetTrDetls(DateTime vStDate, DateTime vEnDate)
		{
			try
			{
				var start = vStDate.Date;
				var endExclusive = vEnDate.Date;
				var result = await (from det in _dbContext.TrDetails
									join head in _dbContext.TrHeads
									on det.TrdTrhId equals head.TrhId
									where head.TrhDate >= start && head.TrhDate < endExclusive && head.TrhApproved==true
									orderby head.TrhDate
									select det)
					.AsNoTracking()
					.ToListAsync();

				return result;
			}
			catch
			{
				throw;
			}
		}


		public async Task<List<TrDetail>> GetTrDetls()
        {
            try
            {
				//return await _dbContext.TrDetails.Where(x=>x.).AsNoTracking().ToListAsync();
				var result = await (from det in _dbContext.TrDetails
									join head in _dbContext.TrHeads
										on det.TrdTrhId equals head.TrhId
									where head.TrhApproved == true
									orderby head.TrhDate
									select det)
					.AsNoTracking()
					.ToListAsync();

				return result;
			}
            catch
            {
                throw;
            }
        }

        public async Task<List<TrDetail>> GetTrDetlsByTrHeadId(long Trhid)
        {
            try
            {
                return await _dbContext.TrDetails.Where(x => x.TrdTrhId == Trhid).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }


        public async Task<string> UpdateTrDetl(TrDetail updatedTrDetl)
        {
            try
            {
                TrDetail? th1 = await _dbContext.TrDetails.Where(x => x.TrdId == updatedTrDetl.TrdId).FirstOrDefaultAsync();
                if (th1 != null)
                {
                    th1.TrdTrhId = updatedTrDetl.TrdTrhId;
                    th1.TrdListNo = updatedTrDetl.TrdListNo;
                    th1.TrdLotNo = updatedTrDetl.TrdLotNo;
                    th1.TrdExpiryDate = updatedTrDetl.TrdExpiryDate;
                    th1.TrdBatchId = updatedTrDetl.TrdBatchId;
                    th1.TrdStkIdDesc = updatedTrDetl.TrdStkIdDesc;
                    th1.TrdStockStkId = updatedTrDetl.TrdStockStkId;
                    th1.TrdClientCodeFrom = updatedTrDetl.TrdClientCodeFrom;
                    th1.TrdClientCodeFromUp = updatedTrDetl.TrdClientCodeFromUp;
                    th1.TrdClientCodeFromQty = updatedTrDetl.TrdClientCodeFromQty;
                    th1.TrdClientCodeTo = updatedTrDetl.TrdClientCodeTo;
                    th1.TrdClientCodeToUp = updatedTrDetl.TrdClientCodeToUp;
                    th1.TrdIdRevJourn = updatedTrDetl.TrdIdRevJourn;
                    th1.TrdAlert = updatedTrDetl.TrdAlert;
                    th1.TrdAlertStop = updatedTrDetl.TrdAlertStop;
                    th1.TrdBalJournQty = updatedTrDetl.TrdBalJournQty ?? 0;
                    th1.TrdLotChange = updatedTrDetl.TrdLotChange;
                    th1.TrdReversal = updatedTrDetl.TrdReversal;
                    th1.TrdRev = updatedTrDetl.TrdRev;
                    th1.TrdRevSeq = updatedTrDetl.TrdRevSeq;
                    th1.TrdHighlightPriceDiff = updatedTrDetl.TrdHighlightPriceDiff;
                    th1.TrdAction = updatedTrDetl.TrdAction;
                    _dbContext.TrDetails.Update(th1);
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

		public async Task UpdateTrdRev()
		{
            try
            {
                await _dbContext.TrDetails
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.TrdRev, false));

				await _dbContext.TrDetails
					.Where(x => x.TrdRevSeq == 0)
					.ExecuteUpdateAsync(setters => setters
						.SetProperty(x => x.TrdRevSeq, x => x.TrdId));

                await _dbContext.TrDetails
                    .Where(x => x.TrdHighlightPriceDiff == true)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(x => x.TrdHighlightPriceDiff,false));

            }
			catch (Exception ex)
			{
                throw;
			}
		}
	}
}
