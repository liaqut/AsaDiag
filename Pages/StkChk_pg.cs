using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using DigiEquipSys.Services;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;
namespace DigiEquipSys.Pages
{
    public partial class StkChk_pg
    {
        [CascadingParameter]
        public EventCallback notify { get; set; }
        private string? myRole;
        private string? myLoc;
        private string? myUser;
        //private ElementReference scanBox;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //await JSRuntime.InvokeVoidAsync("setFocus", scanBox);
                myUser = await sessionStorage.GetItemAsync<string>("adminEmail");
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                await notify.InvokeAsync();
                StateHasChanged();
            }
        }
        public bool SpinnerVisible { get; set; } = false;

        WarningPage? Warning;
        string WarningHeaderMessage = "";
        string WarningContentMessage = "";

        ConfirmPage? DialogDelete;
        private string? ConfirmHeaderMessage = "Confirm Delete";
        private string? ConfirmContentMessage = "Please confirm that you want to Delete this record ";

        ConfirmPage? DialogDeleteAll;
        private string? ConfirmHeaderMessage1 = "Confirm Delete All";
        private string? ConfirmContentMessage1 = "Please confirm that you want to Delete All these records ";

        public long? vRdId { get; set; }

        private SfGrid<StockCheck>? InwardGrid;

        protected List<StockCheck> inwards = new();

        private StockCheck? rcpt;
        private ItemMaster? itemmaster;
        private double? TotrcptQty;

        protected List<ItemMaster> ItemMasterList = new();

        public List<GenScanSpec> genScanSpecList = new();
        //protected Stock currStock=new();
        private GenScanSpec qryspec { get; set; }
        private bool isButtonDisabled = false;
        public long? RcptNo { get; set; }
        private int mysw = 0;
        public string? myLocShort { get; set; }
        public int iSw = 0;
        //private bool isNewRcpt = true;
        private bool isChkList = true;
        private bool vEnable { get; set; } = true;
        //private bool vbuttonEnable { get; set; } = false;

        private SfDialog? DialogAddManual;
        public string? vItemCode { get; set; }
        private SfComboBox<string, ItemMaster> ComboObj;
        private string Custom { get; set; }
        private ElementReference ScanCodeInputRef;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                this.SpinnerVisible = true;
                genScanSpecList = await myScanSpecService.GetGenScanSpecs();
                ItemMasterList = await myItemMaster.GetItemMastersDistinct();
                inwards=await myStockCheckService.GetStockChecks();
                await InvokeAsync(StateHasChanged);
                this.SpinnerVisible = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        public IEditorSettings myEditParams = new NumericEditCellParams
        {
            Params = new NumericTextBoxModel<object>() { ShowClearButton = true, ShowSpinButton = false }
        };

        protected async Task BatchSaveHandler(BeforeBatchSaveArgs<StockCheck> args)
        {
            iSw = 0;
            await InwardGrid.Refresh();
        }
        protected async void CellSavedHandler(CellSavedArgs<StockCheck> args)
        {
            try
            {

                var invdet = args.Data;
                int vLen = 0;
                int vLen1 = 0;
                string xScanCode = "";
                string vCode1 = "";
                switch (args.ColumnName)
                {
                    case "RdScanCode":
                        if (args.Data.RdScanCode != null)
                        {

                            vCode1 = "";
                            vLen = args.Data.RdScanCode.Trim().Length;
                            string vCode = args.Data.RdScanCode.Trim().Substring(vLen - 6, 6);

                            if (args.Data.RdScanCode.Trim().Contains("-00"))
                            {
                                xScanCode = args.Data.RdScanCode.Trim().Replace("-0", "");
                                vLen1 = xScanCode.Trim().Length;
                                vCode1 = xScanCode.Substring(vLen1 - 6, 6);

                            }
                            else
                            {
                                if (args.Data.RdScanCode.Trim().Contains("-0"))
                                {
                                    string xvCode = vCode.Substring(vCode.IndexOf("-"));
                                    if (xvCode.Trim().Length == 4)
                                    {
                                        xScanCode = args.Data.RdScanCode.Trim().Replace("-0", "");
                                    }
                                    else
                                    {
                                        xScanCode = args.Data.RdScanCode.Trim().Replace("-", "");
                                    }
                                    vLen1 = xScanCode.Trim().Length;
                                    vCode1 = xScanCode.Substring(vLen1 - 6, 6);
                                }
                                else
                                {
                                    if (args.Data.RdScanCode.Trim().Contains("-"))
                                    {
                                        xScanCode = args.Data.RdScanCode.Trim().Replace("-", "");
                                        vLen1 = xScanCode.Trim().Length;
                                        vCode1 = xScanCode.Substring(vLen1 - 6, 6);
                                    }
                                }
                            }
                            if (vCode1 == "")
                            {
                                vCode1 = vCode.Trim();
                            }

                            var qrylistNo = ItemMasterList.FirstOrDefault(x => x.ItemListNo == vCode1.ToString());
                            if (qrylistNo != null)
                            {
                                    invdet.Id = 0;
                                    await myStockCheckService.AddStockCheck(invdet);
                            }
                            else
                            {
                                string message = "This " + vCode1 + "is not present in the Master..";
                                await JSRuntime.InvokeVoidAsync("alert", message);
                                break;
                            }
                            await InwardGrid.EndEditAsync();
                            await Task.Delay(100);
                            await AddNewRecord();
                        }
                        break;

                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        private async Task AddNewRecord()
        {
            if (InwardGrid != null)
            {
                try
                {
                    await InwardGrid.AddRecordAsync();
                }
                catch (Exception ex)
                {
                    await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                    return;
                }
            }
            else
            {
                Console.WriteLine("InwardGrid is null.");
            }
        }

        public async Task ProcessList()
        {
            try
            {
                await InwardGrid.EndEditAsync();
                await InwardGrid.Refresh();
                await RcptDetSave();
                NavigationManager.NavigateTo("chkListInv_pg/");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        public async Task DeleteTrans()
        {
            try
            {
                DialogDeleteAll.OpenDialog();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        protected async Task ConfirmDeleteAll(bool DeleteConfirmedAll)
        {
            try
            {
                this.SpinnerVisible = true;
                if (DeleteConfirmedAll)
                {
                    await myStockCheckService.DelStockChecks();
                }
                await InwardGrid.Refresh();
                inwards = await myStockCheckService.GetStockChecks();
                this.SpinnerVisible = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        protected async Task RcptDetSave()
        {
            try
            {
                this.SpinnerVisible = true;
                inwards = await myStockCheckService.GetStockChecks();
                int vLen = 0;
                int vLen1 = 0;
                string xScanCode = "";
                string vCode1 = "";
                rcpt = new StockCheck();
                foreach (var det in inwards)
                {
                    vCode1 = "";
                    vLen = det.RdScanCode.Trim().Length;
                    string vCode = det.RdScanCode.Trim().Substring(vLen - 6, 6);

                    if (det.RdScanCode.Trim().Contains("-00"))
                    {
                        xScanCode = det.RdScanCode.Trim().Replace("-0", "");
                        vLen1 = xScanCode.Trim().Length;
                        vCode1 = xScanCode.Substring(vLen1 - 6, 6);

                    }
                    else
                    {
                        if (det.RdScanCode.Trim().Contains("-0"))
                        {
                            string xvCode = vCode.Substring(vCode.IndexOf("-"));
                            if (xvCode.Trim().Length == 4)
                            {
                                xScanCode = det.RdScanCode.Trim().Replace("-0", "");
                            }
                            else
                            {
                                xScanCode = det.RdScanCode.Trim().Replace("-", "");
                            }
                            vLen1 = xScanCode.Trim().Length;
                            vCode1 = xScanCode.Substring(vLen1 - 6, 6);
                        }
                        else
                        {
                            if (det.RdScanCode.Trim().Contains("-"))
                            {
                                xScanCode = det.RdScanCode.Trim().Replace("-", "");
                                vLen1 = xScanCode.Trim().Length;
                                vCode1 = xScanCode.Substring(vLen1 - 6, 6);
                            }
                        }
                    }
                    if (vCode1 == "")
                    {
                        vCode1 = vCode.Trim();
                    }

                    var qrylistNo = ItemMasterList.FirstOrDefault(x => x.ItemListNo == vCode1.ToString());
                    if (qrylistNo != null)
                    {
                        rcpt.RdScanCode = det.RdScanCode;
                        rcpt.RdListNo = vCode1;
                        rcpt.RdStkId = qrylistNo.ItemId;
                        rcpt.Id = det.Id;
                        rcpt.RdQty = 1;
                        rcpt.RdLotNo = null;
                        rcpt.RdExpiryDate = null;
                        if (vCode.Contains("-"))
                        {
                            if (vCode.Substring(3, 1) == "-")
                            {
                                qryspec = (from y in genScanSpecList where y.GenScanLength == vLen && y.GenType == 2 select y).FirstOrDefault();
                            }
                            else
                            {
                                qryspec = (from y in genScanSpecList where y.GenScanLength == vLen && y.GenType == 3 select y).FirstOrDefault();

                            }
                        }
                        else
                        {
                            qryspec = (from y in genScanSpecList where y.GenScanLength == vLen && y.GenType == 1 select y).FirstOrDefault();
                        }
                        if (qryspec != null)
                        {
                            rcpt.RdLotNo = det.RdScanCode.Substring(qryspec.GenLotStartFrom.Value, qryspec.GenLotLength.Value).ToString();
                            if (qryspec.GenExpiryDir > 0)
                            {
                                var vExp = det.RdScanCode.Substring(qryspec.GenExpiryStartFrom.Value, qryspec.GenExpiryLength.Value).ToString();
                                if (qryspec.GenExpiryDir == 1)
                                {
                                    rcpt.RdExpiryDate = Convert.ToDateTime("20" + vExp.Substring(0, 2) + "-" + vExp.Substring(2, 2) + "-" + vExp.Substring(4, 2));
                                }
                                else
                                {
                                    rcpt.RdExpiryDate = Convert.ToDateTime("20" + vExp.Substring(4, 2) + "-" + vExp.Substring(2, 2) + "-" + vExp.Substring(0, 2));
                                }
                            }
                            await myStockCheckService.UpdateStockCheck(rcpt);
                        }
                        else
                        {
                            await myStockCheckService.UpdateStockCheck(rcpt);
                        }
                    }
                }
                this.SpinnerVisible = false;
                StateHasChanged();
                await InwardGrid.Refresh();

            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        public void RowSelectHandler(RowSelectEventArgs<StockCheck> args)
        {
            vRdId = args.Data.Id;
        }

        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Delete")
            {
                args.Cancel = true;

                if (vRdId > 0)
                    {
                        DialogDelete.OpenDialog();
                    }
                else
                    {
                        Warning.OpenDialog();
                    }
            }
        }

        protected async Task ConfirmDelete(bool DeleteConfirmed)
        {
            try
            {
                this.SpinnerVisible = true;
                if (DeleteConfirmed)
                {
                    await myStockCheckService.DeleteStockCheck(Convert.ToInt64(vRdId));
                }
                await InwardGrid.Refresh();
                inwards = await myStockCheckService.GetStockChecks();

                this.SpinnerVisible = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
    }
}
