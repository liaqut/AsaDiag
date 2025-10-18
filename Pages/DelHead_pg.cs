
using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using DigiEquipSys.Services;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.MultiColumnComboBox;
using Syncfusion.Blazor.Popups;
using System.Drawing.Drawing2D;

namespace DigiEquipSys.Pages
{
    public partial class DelHead_pg
    {
        [CascadingParameter]
        public EventCallback notify { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await notify.InvokeAsync();
                StateHasChanged();
            }
        }

        [Parameter]
        public string? vCompType { get; set; }
        WarningPage? Warning;
        string WarningHeaderMessage = "";
        string WarningContentMessage = "";
        ConfirmPage? DialogDelete;
        private string? ConfirmHeaderMessage = "Confirm Delete";
        private string? ConfirmContentMessage = "Please confirm that you want to Delete this record ";

        private Boolean Disabled = true;
        private Boolean Enabled = false;
        private bool vSave = false;

        public string? myLoc { get; set; }
        public string? myLocShort { get; set; }
        public string? myUser { get; set; }
        private long tmpLineID { get; set; } = 0;
        public bool SpinnerVisible { get; set; } = false;
        public long DelSerNo { get; set; }

        private SfGrid<DelDetl>? DelDetGrid;

        [Parameter]
        public long DelnoteId { get; set; }
        public long vDelnoteId { get; set; }
        public long? vDelDetId { get; set; }

        public DelHead Delnoteaddedit = new DelHead();

        [Inject]
        public IDelHeadService? DelHeadService { get; set; }
        public IEnumerable<DelHead>? Delnotelist;
        [Inject]
        public IDelDetlService? DelDetlService { get; set; }
        protected List<DelDetl> Deldetls = new();
        [Inject]
        public IClientService? ClientService { get; set; }
        public IEnumerable<ClientMaster>? clientlist;
        private GenScanSpec qryspec { get; set; }
        public List<GroupMaster>? ItemGroupList = new();
        public List<CategMaster>? ItemCatList = new();
        public List<ItemUnit>? ItemUnitList = new();
        protected List<ItemMaster> ItemMasterList = new();
        //protected List<ItemMaster> ItemMasterListSelected = new();
        protected List<GenCity>? CityList = new();
        protected List<ClientCity> ClientCityList = new();
        protected List<Branch> companylist = new();
        protected List<Division> branchlist = new();
        public string? GroupDesc { get; set; }
        public string? UnitDesc { get; set; }

        SfDropDownList<long?, ItemMaster> myItem;
        protected ElementReference myQty;
        protected List<Stock> myStockList = new();
        public int iSw = 0;
        private bool isNewDel = true;
        private bool isChkList = true;
        private bool vEnable { get; set; } = true;
        private bool vbuttonEnable { get; set; } = false;
        public string? ddlValue { get; set; }
        SfTextBox myTextBox;
        private string? selectedItem;
        public List<GenScanSpec> genScanSpecList = new();
        private string vClcode { get; set; } = "";
        public class tblClientCity
        {
            public int? ClientId { get; set; }
            public int? CityId { get; set; }
            public string? CityName { get; set; }
        }
        protected List<tblClientCity> myClientCityList = new();
        private SfDialog? DialogAddManual;
        private DelDetl? delv;
        private Stock? stockmaster;
        private ItemMaster? imaster;
        private string mValue { get; set; }
        protected List<StockForJournal> StockForJournalList = new();
        private SfComboBox<int?, ClientMaster> ComboObj;
        private SfComboBox<string?, Branch> Company;
        private string Custom { get; set; }
        //public string? vItemCode { get; set; }
        //private SfComboBox<string?, ItemMaster> ComboObj2;
        public bool IsVisRole { get; set; } = true;
        private string? myRole;
        public int TotalQty { get; set; }
        public decimal TotalAmt { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                vDelnoteId = DelnoteId;
                myUser = await sessionStorage.GetItemAsync<string>("adminEmail");
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                Delnoteaddedit.DelDate = DateTime.Now;
                this.SpinnerVisible = true;
                clientlist = await ClientService.GetClients();
                CityList = await myCityService.GetGenCityDetails();
                ClientCityList = await myClientCityService.GetClientCities();
                genScanSpecList = await myScanSpecService.GetGenScanSpecs();
                if (DelnoteId != 0)
                {
                    Delnoteaddedit = await DelHeadService.GetDelHead(DelnoteId);
                    DelSerNo = (long)Delnoteaddedit.DelId;
                    Deldetls = await DelDetlService.GetDelDetlsByDelNumber(DelSerNo);
                    TotalQty = Convert.ToInt32(Deldetls.Sum(d=>(d.DelQty ?? 0)));
                    TotalAmt = Math.Round(Deldetls.Sum(d => (d.DelQty ?? 0) * (d.DelUprice ?? 0)),2);

                    //isNewDel = false;
                    isChkList = false;
                    if (Delnoteaddedit.DelApproved == true)
                    {
                        vEnable = false;
                        vbuttonEnable = true;
                    }
                    if (Delnoteaddedit.DelClientId !=0)
                    {
                        var myQ = (from x in ClientCityList join y in CityList on x.CityId equals y.CityId select new { x.ClientId, x.CityId, y.CityName }).ToList();
                        myClientCityList = myQ.Where(m => m.ClientId == Delnoteaddedit.DelClientId).Select(g => new tblClientCity { ClientId = g.ClientId, CityId = g.CityId, CityName = g.CityName }).ToList();
                    }
                }
                else
                {
                    Delnoteaddedit.DelComp = "01";   // Default company code
                    Delnoteaddedit.DelBranch = "001";   // Default branch code
                }
                ItemMasterList = await myItemMaster.GetItemMasters();
                ItemGroupList = await myGrpMaster.GetGroupMasters();
                ItemCatList = await myCatMaster.GetCategMasters();
                ItemUnitList = await myUnitMaster.GetItemUnits();
                companylist= await myBranchService.GetBranches();
                branchlist = await myDivisionService.GetDivisions();
                myRole = await sessionStorage.GetItemAsync<string>("adminRo");
                if (myRole == "01" || myRole == "02" || myRole == "03")
                {
                    IsVisRole = true;
                }
                else
                {
                    IsVisRole = false;
                }
                await InvokeAsync(StateHasChanged);
                this.SpinnerVisible = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        private void myclientcity(ChangeEventArgs<int?, ClientMaster> args)
        {
            var myQ = (from x in ClientCityList join y in CityList on x.CityId equals y.CityId select new { x.ClientId, x.CityId, y.CityName }).ToList();
            myClientCityList = myQ.Where(m=>m.ClientId== args.ItemData.ClientId).Select(g => new tblClientCity { ClientId = g.ClientId, CityId = g.CityId, CityName = g.CityName }).ToList();
        }

        private async Task mybranch(ChangeEventArgs<string?, Branch> args)
        {
            Delnoteaddedit.DelBranch = "";   
            branchlist = await myDivisionService.GetDivisions();
            var qbranchlist = branchlist.Where(c => c.LocBranchCode == args.Value).ToList();
            branchlist = qbranchlist.ToList();
        }
        private bool IsRowEmpty(DelDetl Deldet)
        {
            return  Deldet.DelListNo == null; //string.IsNullOrEmpty(Deldet.DelScanCode) ||
        }

        protected void ActionCompleteHandler(ActionEventArgs<DelDetl> Args)
        {
            if (iSw == 0)
            {
                iSw = 1;
                Deldetls = Deldetls.Where(Deldet => !IsRowEmpty(Deldet)).ToList();
            }
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
                Delnoteaddedit.DelDate = DateTime.Now;
                return;
            }
        }
        protected async Task BatchSaveHandler(BeforeBatchSaveArgs<DelDetl> args)
        {
            iSw = 0;
            await DelDetGrid.Refresh();
        }
        public async Task Cancel()
        {
            await PoVouSave();
            NavigationManager.NavigateTo($"Del_pg/{vCompType}");
        }

        public async Task CheckList()
        {
            await PoVouSave();
            NavigationManager.NavigateTo("checkListDel_pg/" + DelnoteId);
        }

        public void RowSelectHandler(RowSelectEventArgs<DelDetl> args)
        {
            vDelDetId = args.Data.DelDetId;
            DelnoteId = Convert.ToInt64(args.Data.DelHeadId);
        }

        protected async Task PoVouSave()
        {
            try
            {
                await DelDetGrid.EndEditAsync();
                if (Deldetls.Count > 0)
                {
                    if (Delnoteaddedit.DelClientId == 0)
                    {
                        WarningHeaderMessage = "Warning!";
                        WarningContentMessage = "Please Select a Client before saving the Delivery Note.";
                        Warning.OpenDialog();
                        return;
                    }
                    if (Delnoteaddedit.DelComp=="" || Delnoteaddedit.DelComp == null)
                    {
                        WarningHeaderMessage = "Warning!";
                        WarningContentMessage = "Please Select a Company before saving the Delivery Note.";
                        Warning.OpenDialog();
                        return;
                    }
                    if (Delnoteaddedit.DelBranch == "" || Delnoteaddedit.DelBranch == null)
                    {
                        WarningHeaderMessage = "Warning!";
                        WarningContentMessage = "Please Select a Branch before saving the Delivery Note.";
                        Warning.OpenDialog();
                        return;
                    }


                }
                isChkList = false;
                this.SpinnerVisible = true;
                await DelHeadService.UpdateDelHead(Delnoteaddedit);
                DelnoteId = Delnoteaddedit.DelId;
                this.SpinnerVisible = false;
                //NavigationManager.NavigateTo($"Del_pg/{vCompType}");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        protected async Task CellSavedHandler(CellSavedArgs<DelDetl> args)
        {
            try
            {
                if (Delnoteaddedit.DelClientId != null)
                {
                    var myQry = await ClientService.GetClient(Convert.ToInt32(Delnoteaddedit.DelClientId));
                    if (myQry != null)
                    {
                        vClcode = myQry.ClientCode;
                    }
                    var invdet = args.Data;
                    int vLen = 0;
                    int vLen1 = 0;
                    string xScanCode = "";
                    string vCode1 = "";
                    switch (args.ColumnName)
                    {
                        case "DelScanCode":
                            if (args.Data.DelScanCode != null)
                            {
                                vCode1 = "";
                                vLen = args.Data.DelScanCode.Trim().Length;
                                string vCode = args.Data.DelScanCode.Trim().Substring(vLen - 6, 6);

                                if (args.Data.DelScanCode.Trim().Contains("-00"))
                                {
                                    xScanCode = args.Data.DelScanCode.Trim().Replace("-0", "");
                                    vLen1 = xScanCode.Trim().Length;
                                    vCode1 = xScanCode.Substring(vLen1 - 6, 6);

                                }
                                else
                                {
                                    if (args.Data.DelScanCode.Trim().Contains("-0"))
                                    {
                                        string xvCode = vCode.Substring(vCode.IndexOf("-"));
                                        if (xvCode.Trim().Length == 4)
                                        {
                                            xScanCode = args.Data.DelScanCode.Trim().Replace("-0", "");
                                        }
                                        else
                                        {
                                            xScanCode = args.Data.DelScanCode.Trim().Replace("-", "");
                                        }
                                        vLen1 = xScanCode.Trim().Length;
                                        vCode1 = xScanCode.Substring(vLen1 - 6, 6);
                                    }
                                    else
                                    {
                                        if (args.Data.DelScanCode.Trim().Contains("-"))
                                        {
                                            xScanCode = args.Data.DelScanCode.Trim().Replace("-", "");
                                            vLen1 = xScanCode.Trim().Length;
                                            vCode1 = xScanCode.Substring(vLen1 - 6, 6);
                                        }
                                    }
                                }
                                if (vCode1 == "")
                                {
                                    vCode1 = vCode;
                                }

                                var qrylistNo = (from x in ItemMasterList where x.ItemListNo == vCode1 && x.ItemClientCode==vClcode select x).FirstOrDefault();  //&& x.ScanCodeLength == vLen
                                if (qrylistNo != null)
                                {
                                    invdet.DelScanCode = args.Data.DelScanCode;
                                    invdet.DelListNo = vCode1;
                                    invdet.DelStkId = qrylistNo.ItemId;
                                    invdet.DelStkIdDesc = qrylistNo.ItemId;
                                    invdet.DelStkIdUnit = qrylistNo.ItemUnit;
                                    invdet.DelStkIdGrp = qrylistNo.ItemGrpCode;
                                    invdet.DelStkIdCat = qrylistNo.ItemCatCode;
                                    invdet.DelClientCode = vClcode;
                                    invdet.DelQty = 1;
                                    invdet.DelUprice = qrylistNo.ItemSellPrice;
									invdet.DelPurchPrice = qrylistNo.ItemCostPrice;

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
                                        invdet.DelLotNo = args.Data.DelScanCode.Substring(qryspec.GenLotStartFrom.Value, qryspec.GenLotLength.Value).ToString();
                                        if (qryspec.GenExpiryDir > 0)
                                        {
                                            var vExp = args.Data.DelScanCode.Substring(qryspec.GenExpiryStartFrom.Value, qryspec.GenExpiryLength.Value).ToString();
                                            if (qryspec.GenExpiryDir == 1)
                                            {
                                                invdet.DelExpiryDate = Convert.ToDateTime("20" + vExp.Substring(0, 2) + "-" + vExp.Substring(2, 2) + "-" + vExp.Substring(4, 2));
                                            }
                                            else
                                            {
                                                invdet.DelExpiryDate = Convert.ToDateTime("20" + vExp.Substring(4, 2) + "-" + vExp.Substring(2, 2) + "-" + vExp.Substring(0, 2));
                                            }
                                        }
                                        //checking for sufficient qty
                                        var qryPrice = await myStockService.GetStockBalance(vCode, invdet.DelLotNo, Convert.ToDateTime(invdet.DelExpiryDate), vClcode);
                                        if (qryPrice != null)
                                        {
                                            if (qryPrice.BalQty >= invdet.DelQty)
                                            {
                                                var qryPrice1 = await myStockService.GetStock(vCode, invdet.DelLotNo, Convert.ToDateTime(invdet.DelExpiryDate), vClcode);
                                                if (qryPrice1 != null)
                                                {
                                                    invdet.DelStockStkId = qryPrice1.StkId;

                                                    if (vDelnoteId == 0)
                                                    {
                                                        Delnoteaddedit.DelId = 0;
                                                        var nextDelNo = await DelHeadService.GetNextDelNoAsync();
                                                        Delnoteaddedit.DelNo = (long)nextDelNo;
                                                        Delnoteaddedit.DelDispNo = "GRN/" + myAccYear.myAccYear + "/" + nextDelNo.ToString().Trim();
                                                        await DelHeadService.CreateDelHead(Delnoteaddedit);

                                                        DelnoteId = Delnoteaddedit.DelId;
                                                        vDelnoteId = Delnoteaddedit.DelId;
                                                        invdet.DelDetId = 0;
                                                        invdet.DelHeadId = DelnoteId;
                                                        await DelDetlService.CreateDelDetl(invdet);
                                                    }
                                                    else
                                                    {
                                                        invdet.DelDetId = 0;
                                                        invdet.DelHeadId = vDelnoteId;
                                                        await DelDetlService.CreateDelDetl(invdet);
                                                    }
                                                }
                                                else
                                                {
                                                    WarningContentMessage = "Specified Item not found in the Stock. Please add to the Stock first...";
                                                    Warning.OpenDialog();
                                                    await Task.Delay(1);
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                invdet.DelQty = 0;
                                                WarningContentMessage = "Qty is Not enough to deliver...";
                                                Warning.OpenDialog();
                                                await Task.Delay(1);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            WarningContentMessage = "Specified Item not found in the Stock. Please add to the Stock first...";
                                            Warning.OpenDialog();
                                            await Task.Delay(1);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        string message = "This is a new type of Scan Code and has to be added in the specification..";
                                        await JSRuntime.InvokeVoidAsync("alert", message);
                                        await Task.Delay(1);
                                        return;
                                    }
                                }
                                else
                                {
                                    string message = "This " + vCode1 + "is not present in the Master..";
                                    await JSRuntime.InvokeVoidAsync("alert", message);
                                    await Task.Delay(1);
                                    return;
                                }
                                await DelDetGrid.EndEditAsync();
                                await Task.Delay(1); // Ensure stable grid state
                                await AddNewRecord();
                            }
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    string message = "Please specify the Client in the Header ..";
                    await JSRuntime.InvokeVoidAsync("alert", message);
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                await Task.Delay(1);
                return;
            }
        }
        private async Task AddNewRecord()
        {
            if (DelDetGrid != null)
            {
                try
                {
                    await DelDetGrid.AddRecordAsync();
                }
                catch (Exception ex)
                {
                    await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                    return;
                }
            }
            else
            {
                Console.WriteLine("Grid is null.");
            }
        }

        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Manual")
            {
                try
                {
					if (vbuttonEnable == true)
					{
						await JSRuntime.InvokeVoidAsync("alert", "This voucher is already approved..You can't add/edit here...");
						return;
					}

					if (Delnoteaddedit.DelClientId != 0 && Delnoteaddedit.DelClientId !=null )
                    {
                        StockForJournalList = await myStockService.GetStockForJournal(Convert.ToInt32(Delnoteaddedit.DelClientId));

                        delv = new DelDetl();
                        delv.DelQty = 1;
                        await this.DialogAddManual.ShowAsync();
                    }
                    else
                    {
                        await JSRuntime.InvokeVoidAsync("alert", "Select a Client First....");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                    return;
                }
            }

            if (args.Item.Text == "Delete")
            {
                args.Cancel = true;
                if (vDelDetId > 0)
                {
                    DialogDelete.OpenDialog();
                }
                else
                {
                    Warning.OpenDialog();
                }
            }

			if (args.Item.Text == "Add")
			{
				if (vbuttonEnable == true)
				{
					WarningHeaderMessage = "Invlaid Addition !";
					WarningContentMessage = "Approved Voucher... You can not Add!!!...";
					Warning.OpenDialog();
				}

			}
		}

        protected async Task ConfirmDelete(bool DeleteConfirmed)
        {
            try
            {
                if (vbuttonEnable == true)
                {
                    await JSRuntime.InvokeVoidAsync("alert", "Approved Voucher..Can't Delete....");
                    return;
                }
                else
                {
                    this.SpinnerVisible = true;
                    if (DeleteConfirmed)
                    {
                        await DelDetlService.DeleteDelDetl(Convert.ToInt64(vDelDetId));
                    }
                    await DelDetGrid.Refresh();
                    Deldetls = await DelDetlService.GetDelDetlsByDelHeadId(DelnoteId);
                    TotalQty = Convert.ToInt32(Deldetls.Sum(d => (d.DelQty ?? 0)));
                    TotalAmt = Math.Round(Deldetls.Sum(d => (d.DelQty ?? 0) * (d.DelUprice ?? 0)),2);
                    this.SpinnerVisible = false;
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        protected async Task DelManualSave()
        {
            try
            {
                this.SpinnerVisible = true;

                if (vDelnoteId == 0)
                {

                    Delnoteaddedit.DelId = 0;
                    var nextDelNo = await DelHeadService.GetNextDelNoAsync();
                    Delnoteaddedit.DelNo = (long)nextDelNo;
                    Delnoteaddedit.DelDispNo = "GRN/" + myAccYear.myAccYear + "/" + nextDelNo.ToString().Trim();

                    await DelHeadService.CreateDelHead(Delnoteaddedit);
                    DelnoteId = Delnoteaddedit.DelId;
                    vDelnoteId = Delnoteaddedit.DelId;
                }
                if (delv.DelDetId == 0)
                {
                    var myQry = await ClientService.GetClient(Convert.ToInt32(Delnoteaddedit.DelClientId));
                    if (myQry != null)
                    {
                        vClcode = myQry.ClientCode;
                    }

                    imaster = await myItemMaster.GetItemMaster(delv.DelListNo, vClcode);
                    if (imaster != null)
                    {
                        delv.DelHeadId = vDelnoteId;
                        delv.DelStkId = imaster.ItemId;
                        delv.DelStkIdDesc = imaster.ItemId;
                        delv.DelStkIdUnit = imaster.ItemUnit;
                        delv.DelStkIdGrp = imaster.ItemGrpCode;
                        delv.DelStkIdCat = imaster.ItemCatCode;
                        delv.DelClientCode = vClcode;
                        delv.DelUprice = imaster.ItemSellPrice;
						//delv.DelPurchPrice = imaster.ItemCostPrice;

						stockmaster = await myStockService.GetStockBatch(delv.DelListNo, delv.DelLotNo, Convert.ToDateTime(delv.DelExpiryDate), vClcode,(int)delv.DelBatchId);
                        if (stockmaster != null)
                        {
                            if (((stockmaster.ItemOpQty ?? 0) + (stockmaster.ItemPurQty ?? 0) + (stockmaster.ItemTrInQty ?? 0) - ((stockmaster.ItemDelQty ?? 0) + (stockmaster.ItemTrOutQty ?? 0))) >= delv.DelQty)
                            {
                                //delv.DelUprice = stockmaster.ItemSp;
                                delv.DelPurchPrice = stockmaster.ItemCp;
                                delv.DelStockStkId = stockmaster.StkId;
                                await DelDetlService.CreateDelDetl(delv);
                                delv = new DelDetl();
                            }
                            else
                            {
                                WarningContentMessage = "Qty is Not enough to deliver...";
                                Warning.OpenDialog();
                            }
                        }
                        else
                        {
                            WarningContentMessage = "Specified Item not found in the Stock. Please add to the Master first...";
                            Warning.OpenDialog();
                        }
                    }
                    else
                    {
                        WarningContentMessage = "This " + delv.DelListNo + "is not present in the Master..";
                        Warning.OpenDialog();
                    }
                }
                Deldetls = await DelDetlService.GetDelDetlsByDelNumber(vDelnoteId);
                StateHasChanged();
                this.SpinnerVisible = false;
                await this.DialogAddManual.HideAsync();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        private async Task CloseDialog()
        {
            await this.DialogAddManual.HideAsync();
        }

        public async Task OnValueChange(ValueChangeEventArgs<string, StockForJournal> args)
        {
            delv.DelListNo = args.ItemData.ItemListNo;
            delv.DelLotNo = args.ItemData.ItemLotNo;
            delv.DelExpiryDate = args.ItemData.ItemExpiryDate;
            delv.DelBatchId = args.ItemData.ItemBatchId;
        }
        private async Task OnFiltering(Syncfusion.Blazor.DropDowns.FilteringEventArgs args)
        {
            Custom = args.Text;
            args.PreventDefaultAction = true;
            var query = new Query().Where(new WhereFilter() { Field = "ClientName", Operator = "contains", value = args.Text, IgnoreCase = true });
            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();
            await ComboObj.FilterAsync(clientlist, query);
        }
        private async Task OnFilteringBranch(Syncfusion.Blazor.DropDowns.FilteringEventArgs args)
        {
            Custom = args.Text;
            args.PreventDefaultAction = true;
            var query1 = new Query().Where(new WhereFilter() { Field = "BranchDesc", Operator = "contains", value = args.Text, IgnoreCase = true });
            query1 = !string.IsNullOrEmpty(args.Text) ? query1 : new Query();
            await Company.FilterAsync(companylist, query1);
        }
        //private async Task OnFiltering2(Syncfusion.Blazor.DropDowns.FilteringEventArgs args)
        //{
        //    Custom = args.Text;
        //    args.PreventDefaultAction = true;
        //    var query = new Query().Where(new WhereFilter() { Field = "ItemListNo", Operator = "contains", value = args.Text, IgnoreCase = true });
        //    query = !string.IsNullOrEmpty(args.Text) ? query : new Query();
        //    await ComboObj2.FilterAsync(ItemMasterList, query);
        //}
        //private async Task OnItemChanged(ChangeEventArgs<string, ItemMaster> args)
        //{
        //    vItemCode = args.Value;
        //    delv.DelListNo = args.ItemData.ItemListNo;
        //    delv.DelClientCode = args.ItemData.ItemClientCode;
        //}
    }
}

