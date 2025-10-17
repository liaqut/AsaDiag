using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DigiEquipSys.Services
{
    public class StockTransService : IStockTransService
    {
        readonly BASS_DBContext _dbContext = new();
        public StockTransService(BASS_DBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<StockTran>> GetStockTransByDate(DateTime vStDate, DateTime vEnDate)
        {
            try
            {
                var start = vStDate.Date;
                var endExclusive = vEnDate.Date;
                var connection = _dbContext.Database.GetDbConnection();
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "CreateStockTrans";
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandTimeout = 240;
                    var startDateParam = command.CreateParameter();
                    startDateParam.ParameterName = "@StartDate";
                    startDateParam.Value = start;
                    command.Parameters.Add(startDateParam);

                    var endDateParam = command.CreateParameter();
                    endDateParam.ParameterName = "@EndDate";
                    endDateParam.Value = endExclusive;
                    command.Parameters.Add(endDateParam);

                    await command.ExecuteNonQueryAsync();
                }
                await connection.CloseAsync();
                return await _dbContext.StockTrans.Where(x => (((x.ItemTrOpQty ?? 0) + (x.ItemPurQty ?? 0) + (x.ItemTrInQty ?? 0)) - ((x.ItemTrOutQty ?? 0) + (x.ItemDelQty ?? 0))) != 0).AsNoTracking().ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task UpdateMultipleTablesAsync(string vCname, string vListNo, string vBatchId, decimal vPurchPrice, decimal vNewPrice)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                await _dbContext.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE Rcpt_Detail SET Rd_Up={vNewPrice} FROM Rcpt_Detail INNER JOIN Rcpt_Head ON Rcpt_Detail.Rd_RhId = Rcpt_Head.Rh_Id INNER JOIN
                Client_Master ON Rcpt_Detail.Rd_ClientCode = Client_Master.Client_Code
                WHERE Rcpt_Head.Rh_Approved=1 and Client_Master.Client_Name={vCname} and Rd_ListNo={vListNo} and Rd_Up={vPurchPrice}");

                await _dbContext.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE Del_Detl SET Del_PurchPrice={vNewPrice} FROM Del_Head INNER JOIN Del_Detl ON Del_Head.Del_Id = Del_Detl.DelHeadId INNER JOIN
                Client_Master ON Del_Detl.Del_ClientCode = Client_Master.Client_Code
                WHERE Del_Head.Del_Approved=1 and Del_BatchId={vBatchId} and Client_Master.Client_Name={vCname} and Del_ListNo={vListNo} and Del_PurchPrice={vPurchPrice}");

                await _dbContext.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE Tr_Detail SET Trd_ClientCode_From_Up={vNewPrice} FROM Tr_Detail INNER JOIN Tr_Head ON Tr_Detail.Trd_TrhId = Tr_Head.Trh_Id INNER JOIN
                Client_Master ON Tr_Detail.Trd_ClientCode_From = Client_Master.Client_Code
                WHERE Tr_Head.Trh_Approved=1 and Trd_BatchId={vBatchId} and Client_Master.Client_Name={vCname} and Trd_ListNo={vListNo} and Trd_ClientCode_From_Up={vPurchPrice}");

                await _dbContext.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE Tr_Detail SET Trd_ClientCode_To_Up={vNewPrice} FROM Tr_Detail INNER JOIN Tr_Head ON Tr_Detail.Trd_TrhId = Tr_Head.Trh_Id INNER JOIN
                Client_Master ON Tr_Detail.Trd_ClientCode_To = Client_Master.Client_Code
                WHERE Tr_Head.Trh_Approved=1 and Trd_BatchId={vBatchId} and Client_Master.Client_Name={vCname} and Trd_ListNo={vListNo} and Trd_ClientCode_To_Up={vPurchPrice}");

                //stock master update
                await _dbContext.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE Stock SET Item_Cp = {vNewPrice}, Item_Pur_Amt = Item_Pur_Qty*{vNewPrice}, Item_TrIn_Amt = Item_TrIn_Qty*{vNewPrice}, Item_TrOut_Amt = Item_TrOut_Qty*{vNewPrice}
                FROM Stock INNER JOIN Client_Master ON Stock.Item_ClientCode = Client_Master.Client_Code
                WHERE Client_Master.Client_Name = {vCname} AND Stock.Item_ListNo = {vListNo} AND Stock.Item_Cp = {vPurchPrice} and Stock.Item_batchId={vBatchId}");

                //for alert
                await _dbContext.Database.ExecuteSqlInterpolatedAsync($@"
                update Tr_Detail set Trd_Alert = 1 FROM Tr_Detail INNER JOIN Tr_Head ON Tr_Detail.Trd_TrhId = Tr_Head.Trh_Id INNER JOIN
                Client_Master ON Tr_Detail.Trd_ClientCode_To = Client_Master.Client_Code 
                where Trd_ClientCode_From_Up > Trd_ClientCode_To_Up and Trd_Alert = 0 and Tr_Head.Trh_Approved=1 and Trd_BatchId={vBatchId} 
                and Client_Master.Client_Name={vCname} and Trd_ListNo={vListNo} and Trd_ClientCode_To_Up={vNewPrice}");

                await _dbContext.Database.ExecuteSqlInterpolatedAsync($@"
                update Tr_Detail set Trd_Alert = 0 FROM Tr_Detail INNER JOIN Tr_Head ON Tr_Detail.Trd_TrhId = Tr_Head.Trh_Id INNER JOIN
                Client_Master ON Tr_Detail.Trd_ClientCode_To = Client_Master.Client_Code 
                where Trd_ClientCode_From_Up <= Trd_ClientCode_To_Up and Trd_Alert = 1 and Tr_Head.Trh_Approved=1 and Trd_BatchId={vBatchId} 
                and Client_Master.Client_Name={vCname} and Trd_ListNo={vListNo} and Trd_ClientCode_To_Up={vNewPrice}");

                //for alert
                await _dbContext.Database.ExecuteSqlInterpolatedAsync($@"
                update Tr_Detail set Trd_Alert = 1 FROM Tr_Detail INNER JOIN Tr_Head ON Tr_Detail.Trd_TrhId = Tr_Head.Trh_Id INNER JOIN
                Client_Master ON Tr_Detail.Trd_ClientCode_From = Client_Master.Client_Code 
                where Trd_ClientCode_From_Up > Trd_ClientCode_To_Up and Trd_Alert = 0 and Tr_Head.Trh_Approved=1 and Trd_BatchId={vBatchId} 
                and Client_Master.Client_Name={vCname} and Trd_ListNo={vListNo} and Trd_ClientCode_To_Up={vNewPrice}");

                await _dbContext.Database.ExecuteSqlInterpolatedAsync($@"
                update Tr_Detail set Trd_Alert = 0 FROM Tr_Detail INNER JOIN Tr_Head ON Tr_Detail.Trd_TrhId = Tr_Head.Trh_Id INNER JOIN
                Client_Master ON Tr_Detail.Trd_ClientCode_From = Client_Master.Client_Code 
                where Trd_ClientCode_From_Up <= Trd_ClientCode_To_Up and Trd_Alert = 1 and Tr_Head.Trh_Approved=1 and Trd_BatchId={vBatchId} 
                and Client_Master.Client_Name={vCname} and Trd_ListNo={vListNo} and Trd_ClientCode_To_Up={vNewPrice}");

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateSmultipleTablesAsync(string vCname, string vListNo, string vBatchId, decimal vSalePrice, decimal vNewPrice)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {

                await _dbContext.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE Del_Detl SET Del_Uprice={vNewPrice} FROM Del_Head INNER JOIN Del_Detl ON Del_Head.Del_Id = Del_Detl.DelHeadId INNER JOIN
                Client_Master ON Del_Detl.Del_ClientCode = Client_Master.Client_Code
                WHERE Del_Head.Del_Approved=1 and Del_BatchId={vBatchId} and Client_Master.Client_Name={vCname} and Del_ListNo={vListNo} and Del_Uprice={vSalePrice}");

                //stock master update
                await _dbContext.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE Stock SET Item_Sp = {vNewPrice}, Item_Del_Amt = Item_Del_Qty*{vNewPrice}
                FROM Stock INNER JOIN Client_Master ON Stock.Item_ClientCode = Client_Master.Client_Code
                WHERE Client_Master.Client_Name = {vCname} AND Stock.Item_ListNo = {vListNo} AND Stock.Item_Sp = {vSalePrice} and Stock.Item_batchId={vBatchId}");

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
