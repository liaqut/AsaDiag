using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using DigiEquipSys.Services;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.MultiColumnComboBox;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace DigiEquipSys.Pages
{
    public partial class StkJournal_pg
    {
        [CascadingParameter]
        public EventCallback notify { get; set; }
        private string? myRole;
        private string? myLoc;
        private string? myUser;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                myUser = await sessionStorage.GetItemAsync<string>("adminEmail");
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                await notify.InvokeAsync();
                StateHasChanged();

            }
        }

        [Parameter]
        public string? vCompType { get; set; }

        WarningPage? Warning;
        string WarningHeaderMessage = "";
        string WarningContentMessage = "";
        public bool SpinnerVisible { get; set; } = false;
        [Parameter] public long TrvouId { get; set; }
        [Parameter] public long myTrdId { get; set; }
        public long TrSerNo { get; set; }
        //public int? VendId { get; set; }
        //public long PohId { get; set; }
        //public string? rcvWhCode { get; set; }
        public long? RcvPoNo { get; set; }
        private SfGrid<TrDetail>? TrDetGrid;
        public List<TrHead>? TrVouList;

        public TrHead trvouaddedit = new();
        public TrDetail detaddedit = new();
        public TrDetail data = new();
        public TrHead trvouaddeditb = new();


        protected List<TrDetail>? trvoudetails = new();
        private TrDetail? tr;
        private long? vTrdId;
        //private double? TotrcptQty;

        //public List<ItemGroup>? ItemGroupList = new();
        //public List<ItemCat>? ItemCatList = new();
        protected List<ClientMaster> ClientList = new();
        protected List<ItemMaster> ItemMasterList = new();
        protected List<Stock> StockList = new();
        protected List<StockForJournal> StockForJournalList = new();
        //public List<GenScanSpec> genScanSpecList = new();
        protected Stock currStock = new();

        //private bool isButtonDisabled = true;
        public long? TrNo { get; set; }
        private int mysw = 0;
        public string? myLocShort { get; set; }
        public int iSw = 0;
        //private bool isNewRcpt = true;
        //private bool isChkList = true;
        private bool vEnable { get; set; } = true;
        private bool vbuttonEnable { get; set; } = false;
        private bool vbuttonEnable2 { get; set; } = false;
        private bool vJourEnable { get; set; } = true;

        //protected List<SupplierMaster> SupplierMasterList = new();
        protected List<ClientMaster>? ClientMasterList { get; set; }
        private string mValue { get; set; }
        private long tmpLineID { get; set; } = 0;
        private long selectedLineID { get; set; } = 0;
        private bool isApprove = true;
        private bool isDisApprove = true;
        private SfComboBox<string, ClientMaster> ComboObj;
        private SfComboBox<string, ClientMaster> ComboObjTo;
        private string Custom { get; set; }
        public string SelectedItemDesc { get; set; }
        public int TotalQty { get; set; }
        public decimal TotalAmt { get; set; }
        public int vJourQty { get; set; } = 0;
        public string vClCode { get; set; }
        public string vItemDesc { get; set; } = "";
        public string vClient { get; set; } = "";
        public bool isChecked { get; set; } = false;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                this.SpinnerVisible = true;
                //isButtonDisabled = true;
                myRole = await sessionStorage.GetItemAsync<string>("adminRo");
                trvouaddedit.TrhDate = DateTime.Now;
                trvouaddedit.TrhDateAltered = DateTime.Now;
                trvouaddedit.TrhUser = myUser;
                //SupplierMasterList = await mySupplierService.GetSuppliers();
                ClientMasterList = await myClientService.GetClients();
                //trvoudetails = await myTrDetailService.GetTrDetls();
                StockList = await myStockService.GetAllStocks();
                //genScanSpecList = await myScanSpecService.GetGenScanSpecs();
                ItemMasterList = await myItemMaster.GetItemMasters();
                StockForJournalList = await myStockService.GetStockForJournal();

                if (TrvouId != 0)
                {
                    trvouaddedit = await myTrHeadService.GetTrHead(TrvouId);
                    TrSerNo = trvouaddedit.TrhId;
                    trvoudetails = await myTrDetailService.GetTrDetlsByTrHeadId(TrSerNo);

                    if (myTrdId != 0)
                    {
                        detaddedit = await myTrDetailService.GetTrDetl(myTrdId);
                        if (detaddedit != null)
                        {
                            vJourEnable = false;
                            if ((detaddedit.TrdBalJournQty ?? 0) != 0)
                            {
                                vJourQty = (int)detaddedit.TrdBalJournQty;
                            }
                            else
                            {
                                vJourQty = (int)(detaddedit.TrdClientCodeFromQty ?? 0);
                            }
                            vClCode = detaddedit.TrdClientCodeFrom;
                            vClient = (from c in ClientMasterList where c.ClientCode == detaddedit.TrdClientCodeFrom select c.ClientName).FirstOrDefault() ?? "";
                            vItemDesc = (from m in ItemMasterList where m.ItemId == detaddedit.TrdStkIdDesc select m.ItemDesc).FirstOrDefault() ?? "";
                        }
                    }
                    else
                    {
                        var q = (from v in trvoudetails orderby v.TrdIdRevJourn descending select v).FirstOrDefault();
                        if (q != null)
                        {
                            if (q.TrdIdRevJourn != null)
                            {
                                myTrdId = (long)q.TrdIdRevJourn;
                                detaddedit = await myTrDetailService.GetTrDetl(myTrdId);
                                if (detaddedit != null)
                                {
                                    vJourEnable = false;
                                    if ((detaddedit.TrdBalJournQty ?? 0) != 0)
                                    {
                                        vJourQty = (int)detaddedit.TrdBalJournQty;
                                    }
                                    else
                                    {
                                        vJourQty = (int)(detaddedit.TrdClientCodeFromQty ?? 0);
                                    }
                                    vClCode = detaddedit.TrdClientCodeFrom;
                                    vClient = (from c in ClientMasterList where c.ClientCode == detaddedit.TrdClientCodeFrom select c.ClientName).FirstOrDefault() ?? "";
                                    vItemDesc = (from m in ItemMasterList where m.ItemId == detaddedit.TrdStkIdDesc select m.ItemDesc).FirstOrDefault() ?? "";

                                }
                            }
                        }
                    }

                    TotalQty = Convert.ToInt32(trvoudetails.Sum(d => (d.TrdClientCodeFromQty ?? 0)));
                    TotalAmt = Math.Round(trvoudetails.Sum(d => (d.TrdClientCodeFromQty ?? 0) * (d.TrdClientCodeFromUp ?? 0)), 2);
                    //isButtonDisabled = false;
                    //isNewRcpt = false;
                    //isChkList = false;
                    if (trvouaddedit.TrhApproved == true)
                    {
                        vEnable = false;
                        vbuttonEnable = true;
                        vbuttonEnable2 = true;
                        isApprove = true;
                        isDisApprove = false;
                        //if (myRole == "01")
                        //{
                        //    isDisApprove = false;
                        //}
                    }
                    else
                    {
                        isApprove = true;
                        vbuttonEnable = false;
                        vbuttonEnable2 = false;
                        isDisApprove = true;
                        if (trvouaddedit.TrhRemarks != null)
                        {
                            if (trvouaddedit.TrhRemarks.Trim() != "")
                            {
                                vEnable = false;
                            }
                        }
                    }
                }

                //ItemGroupList = await myItemMaster.GetItemGroups();
                //ItemCatList = await myItemMaster.GetItemCats();
                await InvokeAsync(StateHasChanged);
                this.SpinnerVisible = false;
                await CloseDialog();

            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        public async Task OnValueChange(ValueChangeEventArgs<string, StockForJournal> args)
        {
            try
            {

                var selectedItem = StockForJournalList.FirstOrDefault(item => item.StkId == args.ItemData.StkId);
                if (selectedItem != null)
                {
                    SelectedItemDesc = selectedItem.ItemDesc;
                }
                detaddedit.TrdStockStkId = args.ItemData.StkId;
                detaddedit.TrdListNo = args.ItemData.ItemListNo;
                detaddedit.TrdLotNo = args.ItemData.ItemLotNo;
                detaddedit.TrdExpiryDate = args.ItemData.ItemExpiryDate;
                detaddedit.TrdBatchId = args.ItemData.ItemBatchId;
                var mystk = await myStockService.GetStock(args.ItemData.StkId);
                if (mystk != null)
                {
                    detaddedit.TrdClientCodeFrom = mystk.ItemClientCode;
                    if (vClCode != null)
                    {
                        detaddedit.TrdClientCodeTo = vClCode;
                        detaddedit.TrdIdRevJourn = myTrdId;
                    }
                    detaddedit.TrdStkIdDesc = mystk.ItemId;
                    //var myMast = await myItemMaster.GetItemMaster(mystk.ItemId);
                    var myMast = await myItemMaster.GetItemMaster(args.ItemData.ItemListNo, mystk.ItemClientCode);

                    if (myMast != null)
                    {
                        detaddedit.TrdClientCodeFromUp = myMast.ItemCostPrice;
                    }
                    else
                    {
                        detaddedit.TrdClientCodeFromUp = 0.00M;
                    }
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }


        private async Task FromUpChange(string newValue)
        {
            try
            {
                detaddedit.TrdClientCodeFrom = newValue;
                var myMast2 = await myItemMaster.GetItemMaster(detaddedit.TrdListNo, detaddedit.TrdClientCodeFrom);
                if (myMast2 != null)
                {
                    detaddedit.TrdClientCodeFromUp = myMast2.ItemCostPrice;
                }
                else
                {
                    detaddedit.TrdClientCodeFromUp = 0.00M;
                    detaddedit.TrdClientCodeFrom = "";
                    detaddedit.TrdClientCodeFromQty = 0;
                    await JSRuntime.InvokeVoidAsync("alert", "Zero Purchase Price....");
                    return;
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        private async Task ToUpChange(string newValue)
        {
            try
            {
                detaddedit.TrdClientCodeTo = newValue;
                var myMast3 = await myItemMaster.GetItemMaster(detaddedit.TrdListNo, detaddedit.TrdClientCodeTo);
                if (myMast3 != null)
                {
                    detaddedit.TrdClientCodeToUp = myMast3.ItemCostPrice;
                }
                else
                {
                    detaddedit.TrdClientCodeToUp = 0.00M;
                    detaddedit.TrdClientCodeTo = "";
                    detaddedit.TrdClientCodeFromQty = 0;
                    await JSRuntime.InvokeVoidAsync("alert", "Zero Purchase Price for the Journal Client....");
                    return;
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        private async Task QtyChanged()
        {
            try
            {
                if ((detaddedit.TrdClientCodeTo ?? "") != "" && (detaddedit.TrdClientCodeFrom ?? "") != "")
                {
                    long vStkId = Convert.ToInt64(detaddedit.TrdStockStkId);
                    var mystk = await myStockService.GetStock(vStkId);
                    if (mystk != null)
                    {
                        if (detaddedit.TrdClientCodeFromQty > ((mystk.ItemOpQty ?? 0) + (mystk.ItemPurQty ?? 0) + (mystk.ItemTrInQty ?? 0) - ((mystk.ItemDelQty ?? 0) + (mystk.ItemTrOutQty ?? 0))))
                        {
                            WarningContentMessage = "Insufficient Qty";
                            Warning.OpenDialog();
                            detaddedit.TrdClientCodeFromQty = 0;
                        }
                        else
                        {
                            if (detaddedit.TrdAlert == true && detaddedit.TrdClientCodeFromQty > vJourQty)
                            {
                                WarningContentMessage = "Shouldn't be more than the reversible Qty";
                                Warning.OpenDialog();
                                detaddedit.TrdClientCodeFromQty = 0;
                            }
                        }
                    }
                }
                else
                {
                    WarningContentMessage = "Incomplete Entry.... Fill the To and From Client first... ";
                    Warning.OpenDialog();
                    detaddedit.TrdClientCodeFromQty = 0;
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }

        }
        protected async Task DetSave()
        {
            try
            {
                if ((detaddedit.TrdClientCodeTo ?? "") != "" && (detaddedit.TrdClientCodeFrom ?? "") != "")
                {
                    if (detaddedit.TrdStockStkId != null)
                    {
                        if (detaddedit.TrdId == 0)
                        {
                            if (detaddedit.TrdClientCodeFromQty > 0)
                            {
                                if (detaddedit.TrdClientCodeFrom != detaddedit.TrdClientCodeTo)
                                {
                                    var myMast1 = await myItemMaster.GetItemMaster(detaddedit.TrdListNo, detaddedit.TrdClientCodeTo);
                                    if (myMast1 != null)
                                    {
                                        detaddedit.TrdClientCodeToUp = myMast1.ItemCostPrice;
                                    }
                                    else
                                    {
                                        detaddedit.TrdClientCodeToUp = 0.00M;
                                        await JSRuntime.InvokeVoidAsync("alert", "Stock won't be created as there is no such Item found in the Master....");
                                        return;
                                    }
                                    tmpLineID--;
                                    trvoudetails.Add(new TrDetail
                                    {
                                        TrdTrhId = detaddedit.TrdTrhId,
                                        TrdId = tmpLineID,
                                        TrdListNo = detaddedit.TrdListNo,
                                        TrdLotNo = detaddedit.TrdLotNo,
                                        TrdExpiryDate = detaddedit.TrdExpiryDate,
                                        TrdBatchId = detaddedit.TrdBatchId,
                                        TrdStkIdDesc = detaddedit.TrdStkIdDesc,
                                        TrdStockStkId = detaddedit.TrdStockStkId,
                                        TrdClientCodeFrom = detaddedit.TrdClientCodeFrom,
                                        TrdClientCodeFromUp = detaddedit.TrdClientCodeFromUp,
                                        TrdClientCodeFromQty = detaddedit.TrdClientCodeFromQty,
                                        TrdClientCodeTo = detaddedit.TrdClientCodeTo,
                                        TrdClientCodeToUp = detaddedit.TrdClientCodeToUp,
                                        TrdAlert = detaddedit.TrdClientCodeFromUp > detaddedit.TrdClientCodeToUp,
                                        TrdAlertStop = false,
                                        TrdIdRevJourn = detaddedit.TrdIdRevJourn,
                                        TrdLotChange = trvouaddedit.TrhExcludeAlertAction == true ? "LotChange" : "",
                                        TrdReversal = detaddedit.TrdIdRevJourn > 0 ? "Reversal" : "",
                                        TrdRevSeq = detaddedit.TrdIdRevJourn > 0 ? detaddedit.TrdIdRevJourn : detaddedit.TrdId,
                                        TrdRev = false
                                    });
                                }
                                else
                                {
                                    await JSRuntime.InvokeVoidAsync("alert", "Stock Journal Can not be for the same client....");
                                    return;
                                }
                            }
                            else
                            {
                                await JSRuntime.InvokeVoidAsync("alert", "Invalid Transfer Qty....");
                                return;
                            }
                        }
                        else
                        {
                            var myMast1 = await myItemMaster.GetItemMaster(detaddedit.TrdListNo, detaddedit.TrdClientCodeTo);
                            if (myMast1 != null)
                            {
                                detaddedit.TrdClientCodeToUp = myMast1.ItemCostPrice;
                            }
                            else
                            {
                                detaddedit.TrdClientCodeToUp = 0.00M;
                                await JSRuntime.InvokeVoidAsync("alert", "Stock won't be created as there is no such Item found in the Master....");
                                return;
                            }
                            detaddedit.TrdAlert = detaddedit.TrdClientCodeFromUp > detaddedit.TrdClientCodeToUp;
                            detaddedit.TrdAlertStop = false;
                            if (detaddedit.TrdIdRevJourn > 0)
                            {
                                detaddedit.TrdRevSeq = detaddedit.TrdIdRevJourn;
                                detaddedit.TrdReversal = "Reversal";
                            }
                            else
                            {
                                detaddedit.TrdRevSeq = detaddedit.TrdId;
                                detaddedit.TrdReversal = "";
                            }

                            await this.TrDetGrid.SetRowDataAsync(detaddedit.TrdId, detaddedit);
                            await CloseDialog();
                        }
                        SelectedItemDesc = "";
                        await TrDetGrid.Refresh();
                        await InvokeAsync(StateHasChanged);
                        await TrVouSave();
                        TotalQty = Convert.ToInt32(trvoudetails.Sum(d => (d.TrdClientCodeFromQty ?? 0)));
                        TotalAmt = Math.Round(trvoudetails.Sum(d => (d.TrdClientCodeFromQty ?? 0) * (d.TrdClientCodeFromUp ?? 0)), 2);
                        detaddedit = new TrDetail();
                        vbuttonEnable = false;
                    }
                    else
                    {
                        await TrDetGrid.Refresh();
                    }
                }
                else
                {
                    if (detaddedit.TrdClientCodeFromQty != 0)
                    {
                        WarningContentMessage = "Incomplete Entry.... Fill the To and From Client first... ";
                        Warning.OpenDialog();
                        detaddedit.TrdClientCodeFromQty = 0;
                    }
                }

            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }

        }

        private void ViewJournal()
        {
            NavigationManager.NavigateTo($"viewjournals_pg");
        }


        private async Task CloseDialog()
        {
            await this.TrDetGrid.ClearSelectionAsync();
            await TrDetGrid.Refresh();
            detaddedit = new TrDetail() { };
        }

        protected async Task VouDetDelete()
        {
            try
            {
                if (selectedLineID > 0)
                {
                    //await TrVouSave();
                    //await InvokeAsync(StateHasChanged);
                    await this.TrDetGrid.DeleteRecordAsync();
                    await myTrDetailService.DeleteTrDetl(selectedLineID);
                    trvoudetails = await myTrDetailService.GetTrDetlsByTrHeadId((long)detaddedit.TrdTrhId);
                    await CloseDialog();
                    await InvokeAsync(StateHasChanged);
                    await TrDetGrid.Refresh();
                    TotalQty = Convert.ToInt32(trvoudetails.Sum(d => (d.TrdClientCodeFromQty ?? 0)));
                    TotalAmt = Math.Round(trvoudetails.Sum(d => (d.TrdClientCodeFromQty ?? 0) * (d.TrdClientCodeFromUp ?? 0)), 2);
                    vbuttonEnable = false;
                    isApprove = true;
                }

            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }

        }
        public async Task RowSelectHandler(RowSelectEventArgs<TrDetail> args)
        {
            selectedLineID = args.Data.TrdId;
            try
            {
                var selectedItem = StockForJournalList.FirstOrDefault(item => item.StkId == args.Data.TrdStockStkId);
                if (selectedItem != null)
                {
                    SelectedItemDesc = selectedItem.ItemDesc;
                }
                this.SpinnerVisible = true;
                detaddedit = new TrDetail()
                {
                    TrdId = args.Data.TrdId,
                    TrdTrhId = args.Data.TrdTrhId,
                    TrdListNo = args.Data.TrdListNo,
                    TrdLotNo = args.Data.TrdLotNo,
                    TrdExpiryDate = args.Data.TrdExpiryDate,
                    TrdBatchId = args.Data.TrdBatchId,
                    TrdStkIdDesc = args.Data.TrdStkIdDesc,
                    TrdStockStkId = args.Data.TrdStockStkId,
                    TrdClientCodeFrom = args.Data.TrdClientCodeFrom,
                    TrdClientCodeFromUp = args.Data.TrdClientCodeFromUp,
                    TrdClientCodeFromQty = args.Data.TrdClientCodeFromQty,
                    TrdClientCodeTo = args.Data.TrdClientCodeTo,
                    TrdAlert = args.Data.TrdAlert,
                    TrdAlertStop = args.Data.TrdAlertStop,
                    TrdIdRevJourn = args.Data.TrdIdRevJourn
                    //TrdBalJournQty = args.Data.TrdBalJournQty
                };
                this.SpinnerVisible = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        public void RowDeSelectHandler(RowDeselectEventArgs<TrDetail> args)
        {
            SelectedItemDesc = "";
            detaddedit = new TrDetail();
        }

        public IEditorSettings myEditParams = new NumericEditCellParams
        {
            Params = new NumericTextBoxModel<object>() { ShowClearButton = true, ShowSpinButton = false }
        };
        public void NoToFutureDate(ChangedEventArgs<DateTime?> args)
        {
            if (args.Value > DateTime.Today)
            {
                WarningHeaderMessage = "Invlaid Date!";
                WarningContentMessage = "Future Dates will not be accepted...";
                Warning.OpenDialog();
                trvouaddedit.TrhDate = DateTime.Now;
                return;
            }
        }

        public async Task Cancel()
        {
            await TrVouSave();
            NavigationManager.NavigateTo("stkjourn_pg/");
        }
        private async Task OnFiltering(Syncfusion.Blazor.DropDowns.FilteringEventArgs args)
        {
            Custom = args.Text;
            args.PreventDefaultAction = true;
            var query = new Query().Where(new WhereFilter() { Field = "ClientName", Operator = "contains", value = args.Text, IgnoreCase = true });
            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();
            await ComboObj.FilterAsync(ClientMasterList, query);
        }

        private async Task OnFilteringTo(Syncfusion.Blazor.DropDowns.FilteringEventArgs args)
        {
            Custom = args.Text;
            args.PreventDefaultAction = true;
            var query = new Query().Where(new WhereFilter() { Field = "ClientName", Operator = "contains", value = args.Text, IgnoreCase = true });
            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();
            await ComboObjTo.FilterAsync(ClientMasterList, query);
        }
        protected async Task TrVouSave()
        {
            await TrDetGrid.EndEditAsync();
            this.SpinnerVisible = true;
            if (trvoudetails.Count() == 0)
            {
                await JSRuntime.InvokeVoidAsync("alert", "This voucher will be deleted as there is no entries were found.....");
                await myTrHeadService.DeleteTrHead(TrvouId);
                NavigationManager.NavigateTo("stkjourn_pg/");
            }
            else
            {
                if (trvoudetails != null)
                {
                    if (TrvouId == 0)
                    {
                        trvouaddedit.TrhId = 0;
                        var nextTrhNo = await myTrHeadService.GetNextTrNoAsync();
                        trvouaddedit.TrhNo = (long?)nextTrhNo;
                        trvouaddedit.TrhDispNo = "GRN/" + myAccYear.myAccYear + "/" + nextTrhNo.ToString().Trim();
                        await myTrHeadService.CreateTrHead(trvouaddedit);
                        TrvouId = trvouaddedit.TrhId;
                        foreach (var individualEntry in trvoudetails)
                        {
                            if ((individualEntry.TrdClientCodeTo ?? "") != "" && (individualEntry.TrdClientCodeFrom ?? "") != "")
                            {
                                individualEntry.TrdId = 0;
                                individualEntry.TrdTrhId = TrvouId;
                                try
                                {
                                    await myTrDetailService.CreateTrDetl(individualEntry);

                                }
                                catch (Exception ex)
                                {
                                    await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                                    return;
                                }
                            }
                            else
                            {
                                WarningContentMessage = "Incomplete Entry.... Fill the To and From Client first... ";
                                Warning.OpenDialog();
                            }
                        }
                    }
                    else
                    {
                        try
                        {
                            await myTrHeadService.UpdateTrHead(trvouaddedit);
                            if (trvoudetails.Count > 0)
                            {
                                foreach (var individualEntry in trvoudetails)
                                {
                                    if ((individualEntry.TrdClientCodeTo ?? "") != "" && (individualEntry.TrdClientCodeFrom ?? "") != "")
                                    {

                                        if (individualEntry.TrdId > 0)
                                        {
                                            await myTrDetailService.UpdateTrDetl(individualEntry);
                                        }
                                        else
                                        {
                                            individualEntry.TrdId = 0;
                                            individualEntry.TrdTrhId = TrvouId;
                                            await myTrDetailService.CreateTrDetl(individualEntry);
                                        }
                                    }
                                    else
                                    {
                                        WarningContentMessage = "Incomplete Entry.... Fill the To and From Client first... ";
                                        Warning.OpenDialog();
                                    }
                                }
                            }
                            else
                            {
                                await myTrDetailService.DeleteTrDetl(TrvouId);
                            }
                        }
                        catch (Exception ex)
                        {
                            await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                            return;
                        }
                    }
                }
                else
                {
                    WarningContentMessage = "Nothing to Save";
                    Warning.OpenDialog();
                }
                vbuttonEnable = true;
                isApprove = false;
                this.SpinnerVisible = false;
            }

        }
        public void NavigateToPrevious()
        {
            NavigationManager.NavigateTo($"stkjourn_pg");
        }

        protected async Task DisApprove()
        {
            if (trvoudetails != null)
            {
                try
                {
                    await myStockService.ExecuteWithTransactionAsync(async () =>
                    {
                        bool allStockUpdatesSucceeded = true;
                        bool anyStockUpdated = false;

                        int vQty = 0;
                        foreach (var individualEntry in trvoudetails)
                        {
                            long chkStkId = 0;
                            var myStk2 = await myStockService.GetStockBatch(individualEntry.TrdListNo, individualEntry.TrdLotNo, Convert.ToDateTime(individualEntry.TrdExpiryDate), individualEntry.TrdClientCodeTo, (int)individualEntry.TrdBatchId);
                            if (myStk2 == null)
                            {
                                allStockUpdatesSucceeded = false;
                                throw new Exception("Failed to find stock for List No: " + individualEntry.TrdListNo + " as there is no stock found for the To Client");
                            }
                            else
                            {
                                chkStkId = myStk2.StkId;
                            }
                            var vSaleStock = await myVwSaleService.GetvwSalesStockList(chkStkId);
                            var sb = new StringBuilder();
                            decimal vRdQty = individualEntry.TrdClientCodeFromQty ?? 0;
                            decimal mqty = 0.00M;
                            int dsw = 0; //delivery
                            foreach (var vs in vSaleStock)
                            {
                                mqty = mqty + vs.DelQty.Value;
                                if (mqty - vRdQty >= 0)
                                {
                                    if (vs.DelApproved == true)
                                    {
                                        sb.AppendLine(vs.ItemDesc.Trim() + " for " + vs.ClientName.Trim() + " " + vs.DelDispNo + ", ");
                                    }
                                    dsw = 1;
                                    break;
                                }
                                else
                                {
                                    if (vs.DelApproved == true)
                                    {
                                        sb.AppendLine(vs.ItemDesc.Trim() + " for " + vs.ClientName.Trim() + " " + vs.DelDispNo + ", ");
                                    }
                                }
                            }
                            if (sb.Length > 0)
                            {
                                allStockUpdatesSucceeded = false;
                                throw new InvalidOperationException("You can't Disapprove this Receipt. Already an Outward... " + sb);
                            }
                            if (dsw == 0)
                            {
                                var vTrfStock = await myVwTransferService.GetvwTransfersStockList(chkStkId);
                                var sb1 = new StringBuilder();
                                decimal Trqty = 0.00M;
                                vRdQty = vRdQty - mqty;
                                foreach (var ts in vTrfStock)
                                {
                                    Trqty = Trqty + ts.TrdClientCodeFromQty.Value;
                                    if (Trqty - vRdQty >= 0)
                                    {
                                        if (ts.TrhApproved == true)
                                        {
                                            sb1.AppendLine(ts.ItemDesc.Trim() + " for " + ts.ClientFrom.Trim() + " " + ts.TrhDispNo + ", ");
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        if (ts.TrhApproved == true)
                                        {
                                            sb1.AppendLine(ts.ItemDesc.Trim() + " for " + ts.ClientFrom.Trim() + " " + ts.TrhDispNo + ", ");
                                        }
                                    }
                                }
                                if (sb1.Length > 0)
                                {
                                    allStockUpdatesSucceeded = false;
                                    throw new InvalidOperationException("You can't Disapprove this since it is already a Journal... " + sb1);
                                }
                            }

                            vTrdId = individualEntry.TrdIdRevJourn;
                            vQty = vQty + (int)individualEntry.TrdClientCodeFromQty;
                            var myStk = await myStockService.GetStockBatch(individualEntry.TrdListNo, individualEntry.TrdLotNo, Convert.ToDateTime(individualEntry.TrdExpiryDate), individualEntry.TrdClientCodeFrom, (int)individualEntry.TrdBatchId);
                            if (myStk != null)
                            {
                                var expectedOutQty = (myStk.ItemTrOutQty ?? 0) - individualEntry.TrdClientCodeFromQty;
                                if (myStk.ItemTrOutQty == expectedOutQty) // Only update if not already updated
                                {
                                    throw new Exception($"Stock for List No {individualEntry.TrdListNo} " +
                                                        $"Lot No {individualEntry.TrdLotNo} already updated. " +
                                                        $"Disapproval stopped to avoid duplicate processing.");
                                }
                                myStk.ItemTrOutQty = (myStk.ItemTrOutQty ?? 0) - individualEntry.TrdClientCodeFromQty;
                                myStk.ItemTrOutAmt = (myStk.ItemTrOutAmt ?? 0) - (individualEntry.TrdClientCodeFromQty * individualEntry.TrdClientCodeFromUp);
                                var updated = await myStockService.UpdateStock(myStk);
                                if (updated != "Success")  // You must make UpdateStock return bool
                                {
                                   allStockUpdatesSucceeded = false;
                                   throw new Exception("Stock update failed for List No: " + individualEntry.TrdListNo);
                                }
                            }
                            else
                            {
                                allStockUpdatesSucceeded = false;
                                throw new Exception("Stock update failed for List No: " + individualEntry.TrdListNo);
                            }
                            //var myStk1 = await myStockService.GetStock(Convert.ToInt64(individualEntry.TrdStkIdDesc), individualEntry.TrdClientCodeTo);
                            var myStk1 = await myStockService.GetStockBatch(individualEntry.TrdListNo, individualEntry.TrdLotNo, Convert.ToDateTime(individualEntry.TrdExpiryDate), individualEntry.TrdClientCodeTo, (int)individualEntry.TrdBatchId);
                            if (myStk1 != null)
                            {
                                var expectedInQty = (myStk1.ItemTrInQty ?? 0) - individualEntry.TrdClientCodeFromQty;
                                if (myStk1.ItemTrInQty == expectedInQty) // Only update if not already updated
                                {                       throw new Exception($"Stock for List No {individualEntry.TrdListNo} " +
                                                        $"Lot No {individualEntry.TrdLotNo} already updated. " +
                                                        $"Disapproval stopped to avoid duplicate processing.");
                                }
                                myStk1.ItemTrInQty = (myStk1.ItemTrInQty ?? 0) - individualEntry.TrdClientCodeFromQty;
                                myStk1.ItemTrInAmt = (myStk1.ItemTrInAmt ?? 0) - (individualEntry.TrdClientCodeFromQty * individualEntry.TrdClientCodeToUp);
                                var updated = await myStockService.UpdateStock(myStk1);
                                if (updated != "Success")  // You must make UpdateStock return bool
                                {
                                    allStockUpdatesSucceeded = false;
                                    throw new Exception("Stock update failed for List No: " + individualEntry.TrdListNo);
                                }
                            }
                            else
                            {
                                allStockUpdatesSucceeded = false;
                                throw new Exception("Stock update failed for List No: " + individualEntry.TrdListNo);
                            }
                            anyStockUpdated = true;

                        }
                        if (allStockUpdatesSucceeded && anyStockUpdated && trvouaddedit != null)
                        {
                            if (vTrdId != null)
                            {
                                detaddedit = await myTrDetailService.GetTrDetl((long)vTrdId);
                                if (detaddedit != null)
                                {
                                    detaddedit.TrdAlertStop = false;
                                    detaddedit.TrdBalJournQty = (vJourQty + vQty);
                                    var updated1 = await myTrDetailService.UpdateTrDetl(detaddedit);
                                    if (updated1 != "Success")  // You must make UpdateStock return bool
                                    {
                                        throw new Exception("Update failed for List No: " + detaddedit.TrdListNo);
                                    }
                                }
                                else
                                {
                                    throw new Exception("Critical Error.. Update failed.. ");
                                }
                            }
                            trvouaddedit.TrhApproved = false;
                            var updated = await myTrHeadService.UpdateTrHead(trvouaddedit);
                            if (updated != "Success")  // You must make UpdateStock return bool
                            {
                                throw new Exception("Update failed.. Try Again !!");
                            }
                            isApprove = true;
                            vbuttonEnable = false;
                            vbuttonEnable2 = false;
                            isDisApprove = true;
                            string message = "Disapproved Successfully...";
                            await JSRuntime.InvokeVoidAsync("alert", message);
                        }
                        else
                        {
                            throw new Exception("Update failed.. Try Again !!");
                        }
                    });

                }
                catch (Exception ex)
                {
                    await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                }

            }
        }

        protected async Task Approve()
        {
            if (trvoudetails != null)
            {
                try
                {
                    await myStockService.ExecuteWithTransactionAsync(async () =>
                    {
                        bool allStockUpdatesSucceeded = true;
                        bool anyStockUpdated = false;
                        foreach (var individualEntry1 in trvoudetails)
                        {
                            if (trvouaddedit != null)
                            {
                                individualEntry1.TrdLotChange = trvouaddedit.TrhExcludeAlertAction == true ? "LotChange" : "";
                                var updated1 = await myTrDetailService.UpdateTrDetl(individualEntry1);
                                if (updated1 != "Success")  // You must make UpdateStock return bool
                                {
                                    allStockUpdatesSucceeded = false;
                                    throw new Exception("LotChange Update Error.. ");
                                }
                            }
                            var myStk = await myStockService.GetStockBatch(individualEntry1.TrdListNo, individualEntry1.TrdLotNo, Convert.ToDateTime(individualEntry1.TrdExpiryDate), individualEntry1.TrdClientCodeFrom, (int)individualEntry1.TrdBatchId);
                            if (myStk == null)
                            {
                                allStockUpdatesSucceeded = false;
                                throw new InvalidOperationException("Stock Item not found (Client From)");
                            }

                            decimal availableStock = ((myStk.ItemOpQty ?? 0) + (myStk.ItemPurQty ?? 0) + (myStk.ItemTrInQty ?? 0) - ((myStk.ItemTrOutQty ?? 0) + (myStk.ItemDelQty ?? 0)));
                            if (individualEntry1.TrdClientCodeFromQty > availableStock)
                            {
                                allStockUpdatesSucceeded = false;
                                string message1 = "Insufficient Qty ...You are Transfering " + individualEntry1.TrdClientCodeFromQty + " Where as the available stock is " + availableStock + " in List No: " + individualEntry1.TrdListNo + " Lot No: " + individualEntry1.TrdLotNo + " Expiry: " + individualEntry1.TrdExpiryDate;
                                throw new Exception(message1);
                            }
                            var expectedOutQty = (myStk.ItemTrOutQty ?? 0) + individualEntry1.TrdClientCodeFromQty;
                            if (myStk.ItemTrOutQty == expectedOutQty) // Only update if not already updated
                            {
                                throw new Exception($"Stock for List No {individualEntry1.TrdListNo} " +
                                                    $"Lot No {individualEntry1.TrdLotNo} already updated. " +
                                                    $"Approval stopped to avoid duplicate processing.");
                            }
                            myStk.ItemTrOutQty = (myStk.ItemTrOutQty ?? 0) + individualEntry1.TrdClientCodeFromQty;
                            myStk.ItemTrOutAmt = (myStk.ItemTrOutAmt ?? 0) + (individualEntry1.TrdClientCodeFromQty * individualEntry1.TrdClientCodeFromUp);
                            var updated = await myStockService.UpdateStock(myStk);
                            if (updated != "Success")  // You must make UpdateStock return bool
                            {
                                 allStockUpdatesSucceeded = false;
                                 throw new Exception("Stock update failed for List No: " + individualEntry1.TrdListNo);
                            }
                        }
                        int vQty = 0;
                        foreach (var individualEntry in trvoudetails)
                        {
                            vTrdId = individualEntry.TrdIdRevJourn;
                            vQty = vQty + (int)individualEntry.TrdClientCodeFromQty;
                            var myStk1 = await myStockService.GetStockBatch(individualEntry.TrdListNo, individualEntry.TrdLotNo, Convert.ToDateTime(individualEntry.TrdExpiryDate), individualEntry.TrdClientCodeTo, (int)individualEntry.TrdBatchId);
                            if (myStk1 == null)
                            {
                                currStock.ItemId = Convert.ToInt64(individualEntry.TrdStkIdDesc);
                                currStock.ItemListNo = individualEntry.TrdListNo;
                                currStock.ItemLotNo = individualEntry.TrdLotNo;
                                currStock.ItemExpiryDate = Convert.ToDateTime(individualEntry.TrdExpiryDate);
                                currStock.ItemClientCode = individualEntry.TrdClientCodeTo;
                                currStock.ItemTrInQty = individualEntry.TrdClientCodeFromQty;
                                currStock.ItemTrInAmt = (individualEntry.TrdClientCodeFromQty * individualEntry.TrdClientCodeToUp);
                                currStock.ItemStkIdDesc = individualEntry.TrdStkIdDesc;
                                currStock.ItemBatchId = (int)individualEntry.TrdBatchId;
                                var myItem = await myItemMaster.GetItemMaster(individualEntry.TrdListNo, individualEntry.TrdClientCodeTo);
                                if (myItem != null)
                                {
                                    currStock.StkId = 0;
                                    currStock.ItemStkIdUnit = myItem.ItemUnit;
                                    currStock.ItemStkIdCat = myItem.ItemCatCode;
                                    currStock.ItemStkIdGrp = myItem.ItemGrpCode;
                                    currStock.ItemCp = myItem.ItemCostPrice;
                                    currStock.ItemSp = myItem.ItemSellPrice;
                                    var createdStock = await myStockService.CreateStock(currStock);
                                    if (createdStock == null || createdStock.StkId <= 0)
                                    {
                                        throw new InvalidOperationException("Stock not created.");
                                    }
                                }
                                else
                                {
                                    allStockUpdatesSucceeded = false;
                                    throw new InvalidOperationException("Item Master not found.");
                                    await JSRuntime.InvokeVoidAsync("alert", "Stock won't be created as there is no such Item found in the Master....");
                                }
                            }
                            else
                            {
                                var expectedInQty = (myStk1.ItemTrInQty ?? 0) + individualEntry.TrdClientCodeFromQty;
                                if (myStk1.ItemTrInQty == expectedInQty) // Only update if not already updated
                                {
                                    throw new Exception($"Stock for List No {individualEntry.TrdListNo} " +
                                                        $"Lot No {individualEntry.TrdLotNo} already updated. " +
                                                        $"Approval stopped to avoid duplicate processing.");
                                }
                                myStk1.ItemTrInQty = (myStk1.ItemTrInQty ?? 0) + individualEntry.TrdClientCodeFromQty;
                                myStk1.ItemTrInAmt = (myStk1.ItemTrInAmt ?? 0) + (individualEntry.TrdClientCodeFromQty * individualEntry.TrdClientCodeToUp);
                                var updated = await myStockService.UpdateStock(myStk1);
                                if (updated != "Success")  // You must make UpdateStock return bool
                                {
                                    allStockUpdatesSucceeded = false;
                                    throw new Exception("Stock update failed for List No: " + individualEntry.TrdListNo);
                                }
                            }
                            anyStockUpdated = true;
                        }
                        if (allStockUpdatesSucceeded && anyStockUpdated && trvouaddedit != null)
                        {
                            if (vTrdId != null)
                            {
                                if (vJourQty - vQty > 0)
                                {
                                    detaddedit = await myTrDetailService.GetTrDetl((long)vTrdId);
                                    if (detaddedit != null)
                                    {
                                        detaddedit.TrdAlertStop = false;
                                        detaddedit.TrdBalJournQty = (vJourQty - vQty);
                                        var updated1 = await myTrDetailService.UpdateTrDetl(detaddedit);
                                        if (updated1 != "Success")  // You must make UpdateStock return bool
                                        {
                                            throw new Exception("Update failed for List No: " + detaddedit.TrdListNo);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Critical Error...Journal Entry not found ...");
                                    }
                                }
                                else
                                {
                                    detaddedit = await myTrDetailService.GetTrDetl((long)vTrdId);
                                    if (detaddedit != null)
                                    {
                                        detaddedit.TrdAlertStop = true;
                                        detaddedit.TrdBalJournQty = 0;
                                        var updated2 = await myTrDetailService.UpdateTrDetl(detaddedit);
                                        if (updated2 != "Success")  // You must make UpdateStock return bool
                                        {
                                            throw new Exception("Update failed for List No: " + detaddedit.TrdListNo);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Critical Error...Journal Entry not found ...");
                                    }
                                }
                            }
                            trvouaddedit.TrhApproved = true;
                            var updated = await myTrHeadService.UpdateTrHead(trvouaddedit);
                            if (updated != "Success")  // You must make UpdateStock return bool
                            {
                                throw new Exception("Update failed.. Try Again!!");
                            }
                            isApprove = true;
                            vbuttonEnable = true;
                            isDisApprove = false;

                            if (vJourQty - vQty > 0)
                            {
                                trvouaddeditb = new TrHead();
                                trvouaddeditb.TrhId = 0;
                                trvouaddeditb.TrhRemarks = trvouaddedit.TrhRemarks;
                                var nextTrhNo = await myTrHeadService.GetNextTrNoAsync();
                                trvouaddeditb.TrhNo = (long?)nextTrhNo;
                                trvouaddeditb.TrhDispNo = "GRN/" + myAccYear.myAccYear + "/" + nextTrhNo.ToString().Trim();
                                trvouaddeditb.TrhDate = DateTime.Now;
                                trvouaddeditb.TrhApproved = false;
                                var createdTrHead = await myTrHeadService.CreateTrHead(trvouaddeditb);
                                if (createdTrHead == null || createdTrHead.TrhId<= 0)
                                {
                                    throw new InvalidOperationException("Transaction Header is not created for the balance qty.");
                                }
                                TrvouId = trvouaddeditb.TrhId;

                                data = await myTrDetailService.GetTrDetl((long)vTrdId);
                                if (data != null)
                                {
                                    detaddedit = new TrDetail()
                                    {
                                        TrdId = 0,
                                        TrdTrhId = TrvouId,
                                        TrdListNo = data.TrdListNo,
                                        TrdLotNo = data.TrdLotNo,
                                        TrdExpiryDate = data.TrdExpiryDate,
                                        TrdStkIdDesc = data.TrdStkIdDesc,
                                        TrdStockStkId = data.TrdStockStkId,
                                        TrdClientCodeFrom = "",
                                        TrdClientCodeFromUp = 0.00M,
                                        TrdClientCodeFromQty = (vJourQty - vQty),
                                        TrdClientCodeTo = data.TrdClientCodeFrom,
                                        TrdClientCodeToUp = data.TrdClientCodeFromUp,
                                        TrdAlert = false,
                                        TrdIdRevJourn = (long)myTrdId,
                                        TrdRev = false
                                    };
                                    var createdTrDetail = await myTrDetailService.CreateTrDetl(detaddedit);
                                    if (createdTrDetail == null || createdTrDetail.TrdId <= 0)
                                    {
                                        throw new InvalidOperationException("Transaction Details are not created for the balance qty.");
                                    }
                                    string message1 = "Created another Journal...";
                                    await JSRuntime.InvokeVoidAsync("alert", message1);
                                }
                                else
                                {
                                    throw new InvalidOperationException("Transaction details are not created for the balance qty.");
                                }
                            }
                            string message = "Approved Successfully...";
                            await JSRuntime.InvokeVoidAsync("alert", message);
                            await InvokeAsync(StateHasChanged);
                            vEnable = false;
                            vbuttonEnable = true;
                            vbuttonEnable2 = true;
                            isApprove = true;
                            isDisApprove = false;
                            //if (myRole == "01")
                            //{
                            //    isDisApprove = false;
                            //}
                        }
                        else
                        {
                            throw new Exception("Update failed.. Try Again!!");
                        }
                    });
                }
                catch (Exception ex)
                {
                    await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                }
            }
        }
    }
}