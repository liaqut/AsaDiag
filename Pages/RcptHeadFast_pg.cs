using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
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
    public partial class RcptHeadFast_pg
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

        [Parameter]
        public string? vCompType { get; set; }
        public string vEdit = "No";

        WarningPage? Warning;
        string WarningHeaderMessage = "";
        string WarningContentMessage = "";

        ConfirmPage? DialogDelete;
        private string? ConfirmHeaderMessage = "Confirm Delete";
        private string? ConfirmContentMessage = "Please confirm that you want to Delete this record ";

        public bool SpinnerVisible { get; set; } = false;
        [Parameter]
        public long RcptvouId { get; set; }
        public long vRcptvouId { get; set; }
        public long? vRdId { get; set; }
        public long RcptSerNo { get; set; }
        public int? VendId { get; set; }
        public long PohId { get; set; }
        public string? rcvWhCode { get; set; }
        public long? RcvPoNo { get; set; }
        private SfGrid<RcptDetail>? RcptDetGrid;

        [Inject]
        public IRcptHeadService? RcptHeadService { get; set; }
        public IEnumerable<RcptHead>? RcptVouList;
        public RcptHead rcptvouaddedit = new();

        [Inject]
        public IRcptDetailService? RcptDetailService { get; set; }
        protected List<RcptDetail> rcptvoudetails = new();
        private RcptDetail? rcpt;
        private ItemMaster? itemmaster;
        private double? TotrcptQty;

        public List<GroupMaster>? ItemGroupList = new();
        public List<CategMaster>? ItemCatList = new();
        public List<ItemUnit>? ItemUnitList = new();
        protected List<Branch> companylist = new();
        protected List<Division> branchlist = new();

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

        protected List<SupplierMaster> SupplierMasterList = new();
        protected List<ClientMaster> ClientMasterList = new();

        private SfDialog? DialogAddManual;
        public string? vItemCode { get; set; }
        private SfComboBox<string, ItemMaster> ComboObj;
        private SfComboBox<string?, Branch> Company;
        private string Custom { get; set; }
        private ElementReference ScanCodeInputRef;
        public int TotalItems { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                this.SpinnerVisible = true;
                //isButtonDisabled = true;
                rcptvouaddedit.RhDate = DateTime.Now;
                rcptvouaddedit.RhDateAltered = DateTime.Now;
                rcptvouaddedit.RhUser = myUser;
                SupplierMasterList = await mySupplierService.GetSuppliers();
                ClientMasterList = await myClientService.GetClients();
                companylist = await myBranchService.GetBranches();
                branchlist = await myDivisionService.GetDivisions();
                genScanSpecList = await myScanSpecService.GetGenScanSpecs();
                if (RcptvouId != 0)
                {
                    vRcptvouId = RcptvouId;
                    rcptvouaddedit = await RcptHeadService.GetRcptHead(RcptvouId);
                    RcptSerNo = rcptvouaddedit.RhId;
                    rcptvoudetails = await RcptDetailService.GetRcptDetails(RcptSerNo);
                    TotalItems = rcptvoudetails.Count();
                    isChkList = false;
                    if (rcptvouaddedit.RhApproved == true)
                    {
                        vEnable = false;
                        isButtonDisabled = true;
                    }
                    else
                    {
                        isButtonDisabled = false;

                        var onetimeQry = (from q in rcptvoudetails where !string.IsNullOrEmpty(q.RdClientCode) select q).FirstOrDefault();
                        if (onetimeQry != null)
                        {
                            WarningHeaderMessage = "Caution !!!";
                            WarningContentMessage = "You already have allocated customers in this GRN... If you Sacn and process it you will loose the allocated customer. You can proceed with Edit instead of EditFast...";
                            Warning.OpenDialog();
                        }

                    }
                }
                else
                {
                    rcptvouaddedit.RhComp = "01";   // Default company code
                    rcptvouaddedit.RhBranch = "001";   // Default branch code
                }
                ItemMasterList = await myItemMaster.GetItemMastersDistinct();
                ItemGroupList = await myGrpService.GetGroupMasters();
                ItemCatList = await myCatService.GetCategMasters();
                ItemUnitList = await myUnitService.GetItemUnits();
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
        public void NoToFutureDate(ChangedEventArgs<DateTime?> args)
        {
            if (args.Value > DateTime.Today)
            {
                WarningHeaderMessage = "Invlaid Date!";
                WarningContentMessage = "Future Dates will not be accepted...";
                Warning.OpenDialog();
                rcptvouaddedit.RhDate = DateTime.Now;
                return;
            }
        }
        private async Task mybranch(ChangeEventArgs<string?, Branch> args)
        {
            rcptvouaddedit.RhBranch = "";
            branchlist = await myDivisionService.GetDivisions();
            var qbranchlist = branchlist.Where(c => c.LocBranchCode == args.Value).ToList();
            branchlist = qbranchlist.ToList();
        }

        protected async Task BatchSaveHandler(BeforeBatchSaveArgs<RcptDetail> args)
        {
            iSw = 0;
            await RcptDetGrid.Refresh();
        }
        protected async void CellSavedHandler(CellSavedArgs<RcptDetail> args)
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
                                if (vRcptvouId == 0)
                                {
                                    //RcptVouList = await RcptHeadService.GetRcptHeads();
                                    //var Qry = (from vou in RcptVouList.OrderByDescending(x => x.RhNo) select vou).FirstOrDefault();
                                    //if (Qry == null)
                                    //{
                                    //    rcptvouaddedit.RhNo = 1000;
                                    //}
                                    //else
                                    //{
                                    //    rcptvouaddedit.RhNo = Qry.RhNo + 1;
                                    //}
                                    //rcptvouaddedit.RhId = 0;
                                    //rcptvouaddedit.RhDispNo = "GRN/" + myAccYear.myAccYear + "/" + (Convert.ToString(rcptvouaddedit.RhNo)).Trim();

                                    rcptvouaddedit.RhId = 0;
                                    var nextRhNo = await RcptHeadService.GetNextRhNoAsync();
                                    rcptvouaddedit.RhNo = (long?)nextRhNo;
                                    rcptvouaddedit.RhDispNo = "GRN/" + myAccYear.myAccYear + "/" + nextRhNo.ToString().Trim();

                                    await RcptHeadService.AddRcptHead(rcptvouaddedit);
                                    RcptvouId = rcptvouaddedit.RhId;
                                    vRcptvouId = rcptvouaddedit.RhId;
                                    invdet.RdId = 0;
                                    invdet.RdRhId = RcptvouId;
                                    await RcptDetailService.AddRcptDetail(invdet);
                                }
                                else
                                {
                                    invdet.RdId = 0;
                                    invdet.RdRhId = vRcptvouId;
                                    await RcptDetailService.AddRcptDetail(invdet);
                                }
                            }
                            else
                            {
                                string message = "This " + vCode1 + "is not present in the Master..";
                                await JSRuntime.InvokeVoidAsync("alert", message);
                                break;
                            }
                            await RcptDetGrid.EndEditAsync();
                            TotalItems = rcptvoudetails.Count;
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
            if (RcptDetGrid != null)
            {
                try
                {
                    await RcptDetGrid.AddRecordAsync();
                }
                catch (Exception ex)
                {
                    await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                    return;
                }
            }
            else
            {
                Console.WriteLine("RcptDetGrid is null.");
            }
        }
        public async Task Cancel()
        {
            try
            {
                await RcptDetGrid.EndEditAsync();
                //rcptvoudetails = rcptvoudetails.Where(rdet => !IsRowEmpty(rdet)).ToList();
                if (isButtonDisabled == false)
                {
                    await RcptVouSave();
                }
                NavigationManager.NavigateTo("rcpt_pg/" + vCompType);
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        public async Task ProcessList()
        {
            try
            {

                if (isButtonDisabled == true)
                {
                    WarningHeaderMessage = "Invlaid Action !";
                    WarningContentMessage = "Approved Voucher... Use Edit Option to see the Approved GRN Detail!!!...";
                    Warning.OpenDialog();
                }
                else
                {
                    await RcptDetGrid.EndEditAsync();
                    await RcptDetGrid.Refresh();
                    if (isButtonDisabled == false)
                    {
                        await RcptVouSave();
                        await RcptDetSave();
                    }
                    NavigationManager.NavigateTo("rcptHead_pg/" + rcptvouaddedit.RhId + "/" + vEdit);
                }
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
                if (rcptvouaddedit.RhComp == "" || rcptvouaddedit.RhComp == null)
                {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please Select a Company before saving the Delivery Note.";
                    Warning.OpenDialog();
                    return;
                }
                if (rcptvouaddedit.RhBranch == "" || rcptvouaddedit.RhBranch == null)
                {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please Select a Branch before saving the Delivery Note.";
                    Warning.OpenDialog();
                    return;
                }
                this.SpinnerVisible = true;
                rcptvoudetails = await RcptDetailService.GetRcptDetails(vRcptvouId);
                int vLen = 0;
                int vLen1 = 0;
                string xScanCode = "";
                string vCode1 = "";
                rcpt = new RcptDetail();
                foreach (var det in rcptvoudetails)
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
                        rcpt.RdStkIdDesc = qrylistNo.ItemId;
                        rcpt.RdStkIdGrp = "";
                        rcpt.RdStkIdCat = "";
                        rcpt.RdStkIdUnit = qrylistNo.ItemUnit;
                        rcpt.RdStkId = qrylistNo.ItemId;
                        rcpt.RdRhId = RcptvouId;
                        rcpt.RdId = det.RdId;
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
                            await RcptDetailService.UpdateRcptDetail(rcpt);
                        }
                        else
                        {
                            await RcptDetailService.UpdateRcptDetail(rcpt);
                        }
                    }
                }
                this.SpinnerVisible = false;
                StateHasChanged();
                await RcptDetGrid.Refresh();

            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        protected async Task RcptVouSave()
        {
            await RcptDetGrid.EndEditAsync();
            await RcptDetGrid.Refresh();
            this.SpinnerVisible = true;
            await RcptHeadService.UpdateRcptHead(rcptvouaddedit);
            this.SpinnerVisible = false;
            //NavigationManager.NavigateTo("rcpt_pg/" + vCompType);
            isChkList = false;
            NavigationManager.NavigateTo("rcptHeadFast_pg/" + rcptvouaddedit.RhId + "/" + vEdit);

        }

        public void RowSelectHandler(RowSelectEventArgs<RcptDetail> args)
        {
            vRdId = args.Data.RdId;
            RcptvouId = Convert.ToInt64(args.Data.RdRhId);
            //vRcptvouId = RcptvouId;
        }

        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Delete")
            {
                args.Cancel = true;

                if (isButtonDisabled == true)
                {
                    WarningHeaderMessage = "Invlaid Deletion !";
                    WarningContentMessage = "Approved Voucher... You can not Delete!!!...";
                    Warning.OpenDialog();
                }
                else
                {
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

            if (args.Item.Text == "Add")
            {
                if (isButtonDisabled == true)
                {
                    WarningHeaderMessage = "Invlaid Addition !";
                    WarningContentMessage = "Approved Voucher... You can not Add!!!...";
                    Warning.OpenDialog();
                }
            }
        }
        private async Task OnFilteringBranch(Syncfusion.Blazor.DropDowns.FilteringEventArgs args)
        {
            Custom = args.Text;
            args.PreventDefaultAction = true;
            var query1 = new Query().Where(new WhereFilter() { Field = "BranchDesc", Operator = "contains", value = args.Text, IgnoreCase = true });
            query1 = !string.IsNullOrEmpty(args.Text) ? query1 : new Query();
            await Company.FilterAsync(companylist, query1);
        }
        protected async Task ConfirmDelete(bool DeleteConfirmed)
        {
            try
            {
                this.SpinnerVisible = true;
                if (DeleteConfirmed)
                {
                    await RcptDetailService.DeleteRcptDetail(Convert.ToInt64(vRdId));
                }
                await RcptDetGrid.Refresh();
                rcptvoudetails = await RcptDetailService.GetRcptDetails(RcptvouId);
                TotalItems = rcptvoudetails.Count;

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
