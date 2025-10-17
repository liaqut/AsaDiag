using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using DigiEquipSys.Services;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.JSInterop;
using SkiaSharp;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Popups;
using Syncfusion.DocIO.DLS;
using System.Globalization;
using System.Security.Cryptography.Xml;
using static DigiEquipSys.Pages.Index;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DigiEquipSys.Pages
{
    public partial class ViewJournals_pg
    {
        [CascadingParameter]
        public EventCallback notify { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await notify.InvokeAsync();
            }
        }

        WarningPage? Warning;
        string WarningHeaderMessage = "Warning!";
        string WarningContentMessage = "You must select a Warehouse";
        public bool SpinnerVisible { get; set; } = false;
        private SfGrid<VwTransfer>? TransferGrid;
        public List<VwTransfer>? TransferList = new();
		public List<VwTransfer>? TransferListAll = new();
		public List<TrDetail>? TrDetailList = new();
		public List<TrDetail>? TrDetailListSelect = new();
		public List<TrHead>? TrHeadList = new();
        private long stkid;
        private string? myLoc;
        public bool IsVisRole { get; set; } = true;
        private string? myRole;
        public int TotalQty { get; set; }
        public decimal TotalAmt { get; set; }
        private SfDialog Dialog;
        public bool IsVisible { get; set; } = false;

        public TrHead trvouaddedita = new();
        public TrDetail detaddedita = new();
        public long TrvouId { get; set; }
        private bool IsDialogVisible = false;
        private List<string> Items = new();
        public long myTrdId = 0;
        public string mysw = "1";
        public bool myRevSw = false;
        public decimal vUp1 = 0.00M;
        public decimal vUp2 = 0.00M;
        private long vTrdRevSeq = 0;
        public class DateRangeSelection
		{
			public DateTime? selectedRangeFrom { get; set; }
			public DateTime? selectedRangeTo { get; set; }
		}
        private DateRangeSelection selectedRange { get; set; } = new DateRangeSelection();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                myRole = await sessionStorage.GetItemAsync<string>("adminRo");
                if (myRole == "01" || myRole == "02")
                {
                    IsVisRole = true;
                }
                else
                {
                    IsVisRole = false;
                }

                this.SpinnerVisible = true;
                string datestringfrom8 = "01/" + DateTime.Now.Month.ToString("00") + "/" + DateTime.Now.Year;
                selectedRange.selectedRangeFrom = DateTime.ParseExact(datestringfrom8, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                selectedRange.selectedRangeTo = DateTime.Now;
                DateTime StDate = selectedRange.selectedRangeFrom.Value;
                DateTime EnDate = selectedRange.selectedRangeTo.Value;
                await myTrDetailService.UpdateTrdRev();
                var Qry3 = await myTrDetailService.GetTrDetls(StDate.AddDays(0), EnDate.AddDays(1));
                var Qry9 = await myTrDetailService.GetTrDetls();
                var Qry = Qry3.Where(x => x.TrdReversal == "Reversal").ToList();
                var Qry1 = Qry.ToList();
                foreach (var q in Qry1)
                {
                    Qry.AddRange(Qry9.Where(y => y.TrdId == q.TrdIdRevJourn));
                }
                Qry9 = new List<TrDetail>(Qry);
                foreach (var transfer in Qry9)
                {
                    transfer.TrdRev = true;
                    await myTrDetailService.UpdateTrDetl(transfer);
                }
                await InvokeAsync(StateHasChanged);
                await myvwTransferService.UpdateAction();
                await Task.Delay(100);
                TransferList = await myvwTransferService.GetvwTransfersDate(StDate.AddDays(0), EnDate.AddDays(1));
				TrDetailList = await myTrDetailService.GetTrDetls();
				TrHeadList =await myTrHeadService.GetTrHeads();
                await InvokeAsync(StateHasChanged);
                TotalQty = Convert.ToInt32(TransferList.Sum(d => (d.QtyFrom ?? 0)));
                TotalAmt = Math.Round(TransferList.Sum(d => (d.AmtFrom ?? 0)), 2);
                this.SpinnerVisible = false;

            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        private async Task OnlyActionTrue()
        {
            try
            {
                mysw = "2";
                this.SpinnerVisible = true;
                await TransferGrid.ClearFilteringAsync();
                await myTrDetailService.UpdateTrdRev();
                var Qry3b = await myTrDetailService.GetTrDetls();
                var Qry9b = await myTrDetailService.GetTrDetls();
                var Qryb = Qry3b.Where(x => x.TrdReversal == "Reversal").ToList();
                var Qry1b = Qryb.ToList();
                foreach (var q in Qry1b)
                {
                    Qryb.AddRange(Qry9b.Where(y => y.TrdId == q.TrdIdRevJourn));
                }
                Qry9b = new List<TrDetail>(Qryb);
                foreach (var transfer in Qry9b)
                {
                    transfer.TrdRev = true;
                    await myTrDetailService.UpdateTrDetl(transfer);
                }
                await InvokeAsync(StateHasChanged);
                await myvwTransferService.UpdateAction();
                await Task.Delay(100);


                var TransferList9 = await myvwTransferService.GetvwTransfers();
                TransferList = TransferList9.Where(x => x.TrdAction == true && (x.TrdLotChange == "" || x.TrdLotChange == null)).OrderBy(z => z.TrdRevSeq).ToList();
                TrDetailList = await myTrDetailService.GetTrDetls();
                TrHeadList = await myTrHeadService.GetTrHeads();

                TransferGrid.DataSource = TransferList;
                await TransferGrid.Refresh();
                await InvokeAsync(StateHasChanged);

                TotalQty = Convert.ToInt32(TransferList.Sum(d => (d.QtyFrom ?? 0)));
                TotalAmt = Math.Round(TransferList.Sum(d => (d.AmtFrom ?? 0)), 2);
                this.SpinnerVisible = false;

            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }


        }
        private string GetActualFilterValue(ActionEventArgs<VwTransfer> args)
        {
            if (args.Columns != null && args.Columns.Count > 0)
            {
                // Check Predicates (most reliable way in Syncfusion)
                if (args.Columns[0].Predicate != null && args.Columns[0].Predicate.Count() > 0)
                {
                    return args.Columns[0].Predicate[0].ToString() ?? "";
                }

                // Fallback to Value (if Predicates is not available)
                return args.Columns[0].Value?.ToString() ?? "";
            }
            return "";
        }
        private async Task ActionBeginHandler(ActionEventArgs<VwTransfer> args)
        {

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.Filtering)
            {
                myRevSw = false;

                if (args.CurrentFilteringColumn == "TrdReversal")
                {
                    string filterValue = GetActualFilterValue(args);

                    if (filterValue == "o" && args.Columns.Count() == 1)
                    {
                        try
                        {
                            this.SpinnerVisible = true;
                            myRevSw = true;
                            TransferList = await myvwTransferService.GetvwTransfers();
                            TransferList = TransferList.Where(x => x.TrdRev == true && (x.TrdLotChange == "" || x.TrdLotChange == null)).OrderBy(z => z.TrdRevSeq).ToList();
                            mysw = "2";
                            await TransferGrid.ClearFilteringAsync();
                            await InvokeAsync(StateHasChanged);
                            TransferGrid.DataSource = TransferList;
                            await TransferGrid.Refresh();
                            TotalQty = Convert.ToInt32(TransferList.Sum(d => (d.QtyFrom ?? 0)));
                            TotalAmt = Math.Round(TransferList.Sum(d => (d.AmtFrom ?? 0)), 2);
                            mysw = "1";
                            this.SpinnerVisible = false;

                        }
                        catch (Exception ex)
                        {
                            await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                            return;
                        }
                    }
                    else
                    {
                        try
                        {
                            this.SpinnerVisible = true;
                            myRevSw = false;
                            mysw = "2";
                            DateTime StDate = selectedRange.selectedRangeFrom.Value;
                            DateTime EnDate = selectedRange.selectedRangeTo.Value;
                            TransferList = await myvwTransferService.GetvwTransfersDate(StDate.AddDays(0), EnDate.AddDays(1));
                            Task.Delay(100);
                            await TransferGrid.ClearFilteringAsync();
                            await InvokeAsync(StateHasChanged);
                            TransferGrid.DataSource = TransferList;
                            await TransferGrid.Refresh();
                            TotalQty = Convert.ToInt32(TransferList.Sum(d => (d.QtyFrom ?? 0)));
                            TotalAmt = Math.Round(TransferList.Sum(d => (d.AmtFrom ?? 0)), 2);
                            mysw = "1";
                            this.SpinnerVisible = false;

                        }
                        catch (Exception ex)
                        {
                            await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                            return;
                        }

                    }
                }
                if (args.CurrentFilteringColumn == "TrdLotChange")
                {
                    try
                    {
                        this.SpinnerVisible = true;
                        myRevSw = false;
                        DateTime StDate = selectedRange.selectedRangeFrom.Value;
                        DateTime EnDate = selectedRange.selectedRangeTo.Value;
                        TransferList = await myvwTransferService.GetvwTransfersDate(StDate.AddDays(0), EnDate.AddDays(1));
                        TransferList = TransferList.Where(x => x.TrdRev == false && (x.TrdLotChange == "" || x.TrdLotChange == null)).OrderBy(z => z.TrdRevSeq).ToList();
                        mysw = "2";
                        await TransferGrid.ClearFilteringAsync();
                        await InvokeAsync(StateHasChanged);
                        TransferGrid.DataSource = TransferList;
                        await TransferGrid.Refresh();
                        TotalQty = Convert.ToInt32(TransferList.Sum(d => (d.QtyFrom ?? 0)));
                        TotalAmt = Math.Round(TransferList.Sum(d => (d.AmtFrom ?? 0)), 2);
                        mysw = "1";
                        this.SpinnerVisible = false;
                    }
                    catch (Exception ex)
                    {
                        await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                        return;
                    }
                }
                if (args.CurrentFilteringColumn == "TrdAction")
                {
                    try
                    {
                        this.SpinnerVisible = true;
                        myRevSw = false;
                        DateTime StDate = selectedRange.selectedRangeFrom.Value;
                        DateTime EnDate = selectedRange.selectedRangeTo.Value;

                        TransferList = await myvwTransferService.GetvwTransfersDate(StDate.AddDays(0), EnDate.AddDays(1));
                        if ((bool)args.Columns[0].Value == true)
                        {
                            TransferList = TransferList.Where(x => x.TrdAction == true && (x.TrdLotChange == "" || x.TrdLotChange == null)).OrderBy(z => z.TrdRevSeq).ToList();
                        }
                        else
                        {
                            TransferList = TransferList.Where(x => x.TrdAction == false && (x.TrdLotChange == "" || x.TrdLotChange == null)).OrderBy(z => z.TrdRevSeq).ToList();
                        }
                        mysw = "2";
                        await TransferGrid.ClearFilteringAsync();
                        await InvokeAsync(StateHasChanged);
                        TransferGrid.DataSource = TransferList;
                        await TransferGrid.Refresh();
                        TotalQty = Convert.ToInt32(TransferList.Sum(d => (d.QtyFrom ?? 0)));
                        TotalAmt = Math.Round(TransferList.Sum(d => (d.AmtFrom ?? 0)), 2);
                        mysw = "1";
                        this.SpinnerVisible = false;

                    }
                    catch (Exception ex)
                    {
                        await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                        return;
                    }
                }


            }

            if (args.RequestType == Syncfusion.Blazor.Grids.Action.ClearFiltering)
            {
                if (mysw == "1")
                {
                    var args1 = new RangePickerEventArgs<DateTime?>
                    {
                        StartDate = selectedRange.selectedRangeFrom,
                        EndDate = selectedRange.selectedRangeTo
                    };
                    await ValueChangeHandler(args1);
                }

            }
        }
        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Excel Export") //Id is combination of Grid's ID and itemname
            {
                try
                {
                    this.SpinnerVisible = true;
                    if (TransferGrid != null)
                    {
                        await TransferGrid.ExportToExcelAsync();
                    }
                    this.SpinnerVisible = false;
                }
                catch (Exception ex)
                {
                    await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                    return;
                }
            }
        }
        public async Task ValueChangeHandler(RangePickerEventArgs<DateTime?> args)
        {
            try
            {
                this.SpinnerVisible = true;
                myRevSw = false;
                DateTime StDate = args.StartDate.Value;
                DateTime EnDate = args.EndDate.Value;
                await myTrDetailService.UpdateTrdRev();

                var Qry3a = await myTrDetailService.GetTrDetls(StDate.AddDays(0), EnDate.AddDays(1));
                var Qry9a = await myTrDetailService.GetTrDetls();
                var Qrya = Qry3a.Where(x => x.TrdReversal == "Reversal").ToList();
                var Qry1a = Qrya.ToList();
                foreach (var q in Qry1a)
                {
                    Qrya.AddRange(Qry9a.Where(y => y.TrdId == q.TrdIdRevJourn));
                }
                Qry9a = new List<TrDetail>(Qrya);
                foreach (var transfer in Qry9a)
                {
                    transfer.TrdRev = true;
                    await myTrDetailService.UpdateTrDetl(transfer);
                }
                await InvokeAsync(StateHasChanged);
                TransferList = await myvwTransferService.GetvwTransfersDate(StDate.AddDays(0), EnDate.AddDays(1));
                TotalQty = Convert.ToInt32(TransferList.Sum(d => (d.QtyFrom ?? 0)));
                await myvwTransferService.UpdateAction();
                Task.Delay(100);
                TotalAmt = Math.Round(TransferList.Sum(d => (d.AmtFrom ?? 0)), 2);
                TransferGrid.DataSource = TransferList;
                await TransferGrid.Refresh();
                this.SpinnerVisible = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }

        }
        public void NavigateToPrevious()
        {
            UriHelper.NavigateTo($"blank_pg");
        }

        //private void OpenDialog()
        //{
        //    IsDialogVisible = true;
        //}

        private async Task NavigateToItem(string item)
        {
            try
            {
                if (item != "Create New Reversal Journal")
                {
                    TrvouId = (from vx in TrDetailList where vx.TrdIdRevJourn == myTrdId orderby vx.TrdTrhId descending select (long)vx.TrdTrhId).FirstOrDefault();
                    UriHelper.NavigateTo($"stkjournal_pg/{TrvouId}/{myTrdId}");
                }
                else
                {
                    var data1 = (from mx in TransferList where mx.TrdId == myTrdId select mx).FirstOrDefault();
                    trvouaddedita.TrhId = 0;
                    trvouaddedita.TrhRemarks = "Reversing From " + data1.TrhDispNo;
                    var nextTrhNo1 = await myTrHeadService.GetNextTrNoAsync();
                    trvouaddedita.TrhNo = (long?)nextTrhNo1;
                    trvouaddedita.TrhDispNo = "GRN/" + myAccYear.myAccYear + "/" + nextTrhNo1.ToString().Trim();
                    trvouaddedita.TrhDate = DateTime.Now;
                    trvouaddedita.TrhApproved = false;
                    await myTrHeadService.CreateTrHead(trvouaddedita);
                    TrvouId = trvouaddedita.TrhId;
                    detaddedita = new TrDetail()
                    {
                        TrdId = 0,
                        TrdTrhId = TrvouId,
                        TrdListNo = data1.TrdListNo,
                        TrdLotNo = data1.TrdLotNo,
                        TrdExpiryDate = data1.TrdExpiryDate,
                        TrdBatchId = data1.TrdBatchId,
                        TrdStkIdDesc = data1.TrdStkIdDesc,
                        TrdStockStkId = data1.TrdStockStkId,
                        TrdClientCodeFrom = "",
                        TrdClientCodeFromUp = 0.00M,
                        TrdClientCodeFromQty = data1.QtyFrom,
                        TrdClientCodeTo = data1.TrdClientCodeFrom,
                        TrdClientCodeToUp = data1.TrdClientCodeFromUp,
                        TrdAlert = false,
                        TrdIdRevJourn = (long)myTrdId,
                        TrdReversal = "Reversal",
                        TrdBalJournQty = 0,
                        TrdRevSeq = (long)myTrdId,
                        TrdRev = false
                    };
                    await myTrDetailService.CreateTrDetl(detaddedita);
                    UriHelper.NavigateTo($"stkjournal_pg/{TrvouId}/{myTrdId}");
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        private void CloseDialog()
        {
            IsDialogVisible = false;
        }

        private void GotoStkJournal()
        {
            UriHelper.NavigateTo($"stkjourn_pg");

        }
        private async Task OnButtonClick(VwTransfer data)
        {
            try
            {
                Items.Clear();
                detaddedita = await myTrDetailService.GetTrDetl(data.TrdId);

                if (detaddedita != null)
                {
                    myTrdId = data.TrdId;
                    var qry = (from vx in TrDetailList join vy in TrHeadList on vx.TrdTrhId equals vy.TrhId  where vx.TrdIdRevJourn == myTrdId select vy).ToList();
                    if (qry != null && qry.Count()>0)
                    {
                        foreach (var q in qry)
                        {
                            if (q.TrhApproved == true)
                            {
                                Items.Add(q.TrhDispNo.ToString()+ " - Approved");
                            }
                            else
                            {
                                Items.Add(q.TrhDispNo.ToString() + " - Pending");
                            }
                        }
                        Items.Add("Create New Reversal Journal");
                        IsDialogVisible = true;
                    }
                    else
                    {
                        trvouaddedita.TrhId = 0;
                        trvouaddedita.TrhRemarks = "Reversing From " + data.TrhDispNo;
                        var nextTrhNo = await myTrHeadService.GetNextTrNoAsync();
                        trvouaddedita.TrhNo = (long?)nextTrhNo;
                        trvouaddedita.TrhDispNo = "GRN/" + myAccYear.myAccYear + "/" + nextTrhNo.ToString().Trim();
                        trvouaddedita.TrhDate = DateTime.Now;
                        trvouaddedita.TrhApproved = false;
                        await myTrHeadService.CreateTrHead(trvouaddedita);
                        TrvouId = trvouaddedita.TrhId;
                        detaddedita = new TrDetail()
                        {
                            TrdId = 0,
                            TrdTrhId = TrvouId,
                            TrdListNo = data.TrdListNo,
                            TrdLotNo = data.TrdLotNo,
                            TrdExpiryDate = data.TrdExpiryDate,
                            TrdBatchId=data.TrdBatchId,
                            TrdStkIdDesc = data.TrdStkIdDesc,
                            TrdStockStkId = data.TrdStockStkId,
                            TrdClientCodeFrom = "",
                            TrdClientCodeFromUp = 0.00M,
                            TrdClientCodeFromQty = data.QtyFrom,
                            TrdClientCodeTo = data.TrdClientCodeFrom,
                            TrdClientCodeToUp = data.TrdClientCodeFromUp,
                            TrdAlert = false,
                            TrdIdRevJourn = (long)myTrdId,
                            TrdReversal="Reversal",
                            TrdBalJournQty=0,
                            TrdRevSeq = (long)myTrdId,
                            TrdRev=false
                        };
                        await myTrDetailService.CreateTrDetl(detaddedita);
                        UriHelper.NavigateTo($"stkjournal_pg/{TrvouId}/{myTrdId}");
                    }
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }

        }
        public void OnRowDataBound(RowDataBoundEventArgs<VwTransfer> args)
        {
            try
            {
                if (myRevSw == true)
                {
                    if (args.Data.TrdHighlightPriceDiff == true)
                    {
                        args.Row.AddClass(new string[] { "highlight-up" });
                    }
                }
                else
                {
                    if (args.Data.TrdAction == true ) //&& args.Data.TrdAlertStop == false && (args.Data.TrdLotChange == null || args.Data.TrdLotChange == ""))
                    {
                        args.Row.AddClass(new string[] { "highlight-row" });
                    }
                }
            }
            catch (Exception ex)
            {
                JSRuntime.InvokeVoidAsync("alert", ex.Message);
            }
        }
    }
}
