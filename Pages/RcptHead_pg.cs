using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Popups;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DigiEquipSys.Pages
{
    public partial class RcptHead_pg
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
        private bool isChkList = false;
        private bool vEnable { get; set; } = true;
        //private bool vbuttonEnable { get; set; } = false;

        protected List<SupplierMaster> SupplierMasterList = new();
        protected List<ClientMaster> ClientMasterList = new();
        private SfComboBox<string?, Branch> Company;

        private SfDialog? DialogAddManual;
        public string? vItemCode { get; set; }
        private SfComboBox<string, ItemMaster> ComboObj;
        private string Custom { get; set; }
        public bool fillsw = false;
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

                    //isButtonDisabled = false;
                    //isNewRcpt = false;
                    isChkList = false;
                    if (rcptvouaddedit.RhApproved == true)
                    {
                        vEnable = false;
                        isButtonDisabled = true;
                        //vbuttonEnable = true;
                    }
                    else
                    {
                        isButtonDisabled = false;
                    }
                }
                else
                {
                    rcptvouaddedit.RhComp = "01";   // Default company code
                    rcptvouaddedit.RhBranch = "001";   // Default branch code
                }

                ItemMasterList = await myItemMaster.GetItemMasters();

                //foreach (var item in ItemMasterList)
                //{
                //    Console.WriteLine($"ItemListNo: {item.ItemListNo}");
                //}

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
        //private async Task OnActionBegin(ActionEventArgs<RcptDetail> args)
        //{
        //    if (args.RequestType == Syncfusion.Blazor.Grids.Action.Filtering)
        //    {
        //        var filteredRecords = await RcptDetGrid.GetCurrentViewRecordsAsync();
        //        TotalItems = filteredRecords.Count;
        //        StateHasChanged();
        //    }
        //}
        private async Task OnItemChanged(ChangeEventArgs<string, ItemMaster> args)
        {
            vItemCode = args.Value;
            rcpt.RdListNo = args.Value;
        }
        private bool IsRowEmpty(RcptDetail rdet)
        {
            return rdet.RdListNo == null;
            //return string.IsNullOrEmpty(rdet.RdScanCode) || rdet.RdListNo == null;
        }

        protected void ActionCompleteHandler(ActionEventArgs<RcptDetail> Args)
        {
            if (iSw == 0)
            {
                iSw = 1;
                rcptvoudetails = rcptvoudetails.Where(rdet => !IsRowEmpty(rdet)).ToList();
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
                string xScanCode="";
                string vCode1="";
                int log = 0;
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
                                        log = 1;
                                    }
                                }
                            }
                            if (vCode1=="")
                            {
                                vCode1 = vCode.Trim();
                            }

                            var qrylistNo = ItemMasterList.FirstOrDefault(x => x.ItemListNo == vCode1.ToString());
                            //qrylistNo = await myItemMaster.GetItemMaster(vCode1);
                            if (qrylistNo != null)
                            {
                                invdet.RdScanCode = args.Data.RdScanCode;
                                invdet.RdListNo = vCode1;
                                invdet.RdStkIdDesc = qrylistNo.ItemId;
                                invdet.RdStkIdGrp = qrylistNo.ItemGrpCode;
                                invdet.RdStkIdCat = qrylistNo.ItemCatCode;
                                invdet.RdStkIdUnit = qrylistNo.ItemUnit;
                                invdet.RdStkId = qrylistNo.ItemId;
                                invdet.RdQty = 1;
                                if (vCode.Contains("-"))
                                {
                                    if (log == 1)
                                    {
                                        qryspec = (from y in genScanSpecList where y.GenScanLength == vLen && y.GenType == 3 select y).FirstOrDefault();

                                    }
                                    else
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
                                }
                                else
                                {
                                    qryspec = (from y in genScanSpecList where y.GenScanLength == vLen && y.GenType == 1 select y).FirstOrDefault();
                                }
                                if (qryspec != null)
                                {
                                    invdet.RdLotNo = args.Data.RdScanCode.Substring(qryspec.GenLotStartFrom.Value, qryspec.GenLotLength.Value).ToString();
                                    if (qryspec.GenExpiryDir > 0)
                                    {
                                        var vExp = args.Data.RdScanCode.Substring(qryspec.GenExpiryStartFrom.Value, qryspec.GenExpiryLength.Value).ToString();
                                        if (qryspec.GenExpiryDir == 1)
                                        {
                                            invdet.RdExpiryDate = Convert.ToDateTime("20" + vExp.Substring(0, 2) + "-" + vExp.Substring(2, 2) + "-" + vExp.Substring(4, 2));
                                        }
                                        else
                                        {
                                            invdet.RdExpiryDate = Convert.ToDateTime("20" + vExp.Substring(4, 2) + "-" + vExp.Substring(2, 2) + "-" + vExp.Substring(0, 2));
                                        }
                                    }
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
                                    string message = "This is a new type of Scan Code and has to be added in the specification..";
                                    await JSRuntime.InvokeVoidAsync("alert", message);
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
                            }
                            else
                            {
                                string message = "This " + vCode1 + "is not present in the Master..";
                                await JSRuntime.InvokeVoidAsync("alert", message);
                            }
                            await RcptDetGrid.EndEditAsync();
                            TotalItems = rcptvoudetails.Count;

                            await Task.Delay(100);
                            await AddNewRecord();
                        }
                        break;

                    case "RdLotNo":
                        {
                            var qry1 = (from x in rcptvoudetails where x.RdId == invdet.RdId select x).FirstOrDefault(); //&& x.ScanCodeLength == vLen
                            if (qry1 != null)
                            {
                                await RcptDetailService.UpdateRcptDetail(invdet);
                            }
                        }
                        break;

                    case "RdExpiryDate":
                        {
                            var qry1 = (from x in rcptvoudetails where x.RdId == invdet.RdId select x).FirstOrDefault(); //&& x.ScanCodeLength == vLen
                            if (qry1 != null)
                            {
                                await RcptDetailService.UpdateRcptDetail(invdet);
                            }
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
                rcptvoudetails = rcptvoudetails.Where(rdet => !IsRowEmpty(rdet)).ToList();
                if (isButtonDisabled == false)
                {
                    await RcptVouSave();
                }
                if (fillsw == false)
                {
                    NavigationManager.NavigateTo("rcpt_pg/" + vCompType);
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("alert", "few Mandatory fields are empty...");
                    return;
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        public async Task CheckList()
        {
            try
            {
                await RcptDetGrid.EndEditAsync();
                rcptvoudetails = rcptvoudetails.Where(rdet => !IsRowEmpty(rdet)).ToList();
                if (isButtonDisabled == false)
                {
                    await RcptVouSave();
                }
                if (fillsw == false)
                {
                    NavigationManager.NavigateTo("checkList_pg/" + rcptvouaddedit.RhId + "/" + vEdit);
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("alert", "few Mandatory fields are empty...");
                    return;
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
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

        protected async Task RcptVouSave()
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

            await RcptDetGrid.EndEditAsync();
            await RcptDetGrid.Refresh();
            this.SpinnerVisible = true;
            rcptvoudetails = rcptvoudetails.Where(rdet => !IsRowEmpty(rdet)).ToList();
            await RcptHeadService.UpdateRcptHead(rcptvouaddedit);
            StateHasChanged();
            this.SpinnerVisible = false;
            fillsw = false;
            var qry1 = (from y in rcptvoudetails where y.RdRhId == vRcptvouId select y).ToList();
            foreach (var q in qry1)
            {
                if (q.RdLotNo == null || q.RdExpiryDate == null)
                {
                    fillsw = true;
                    break;
                }
            }
        }

        public void RowSelectHandler(RowSelectEventArgs<RcptDetail> args)
        {
            vRdId = args.Data.RdId;
            RcptvouId = Convert.ToInt64(args.Data.RdRhId);
            //vRcptvouId = RcptvouId;
        }

        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Manual")
            {
                try
                {
                    if (isButtonDisabled == true)
                    {
                        await JSRuntime.InvokeVoidAsync("alert", "This voucher is already approved..You can't add/edit here...");
                        return;
                    }

                    rcpt = new RcptDetail();
                    rcpt.RdQty = 1;
                    await this.DialogAddManual.ShowAsync();
                }
                catch (Exception ex)
                {
                    await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                    return;
                }
            }

            if (args.Item.Text == "Add")
            {
                if (isButtonDisabled==true)
                {
                    WarningHeaderMessage = "Invlaid Addition !";
                    WarningContentMessage = "Approved Voucher... You can not Add!!!...";
                    Warning.OpenDialog();
                }

            }

            if (args.Item.Text == "Delete")
            {
                args.Cancel = true;
                if (vRdId > 0)
                {
                    if (!isButtonDisabled == true)
                    {
                        DialogDelete.OpenDialog();
                    }
                    else
                    {
                        WarningHeaderMessage = "Invlaid Delete !";
                        WarningContentMessage = "Approved Voucher... You can not delete!!!...";
                        Warning.OpenDialog();
                    }
                }
                else
                {
                    WarningHeaderMessage = "Invlaid Delete !";
                    WarningContentMessage = "Nothing to delete here...";
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
        protected async Task RcptManualSave()
        {
            try
            {
                if (isButtonDisabled == true)
                {
                    await JSRuntime.InvokeVoidAsync("alert", "This voucher is already approved..You can't add/edit here...");
                    return;
                }

                if (rcpt.RdListNo == null || rcpt.RdLotNo == null || rcpt.RdExpiryDate == null || rcpt.RdQty == 0 || rcpt.RdQty == null)
                {
                    await JSRuntime.InvokeVoidAsync("alert", "You have to fill the required fields...");
                    return;
                }

                this.SpinnerVisible = true;

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

                }
                if (rcpt.RdId == 0)
                {
                    itemmaster = await myItemMaster.GetItemMaster(rcpt.RdListNo);
                    if (itemmaster != null)
                    {
                        rcpt.RdRhId = RcptvouId;
                        rcpt.RdStkIdDesc = itemmaster.ItemId;
                        rcpt.RdStkIdGrp = itemmaster.ItemGrpCode;
                        rcpt.RdStkIdCat = itemmaster.ItemCatCode;
                        rcpt.RdStkIdUnit = itemmaster.ItemUnit;
                        rcpt.RdStkId = itemmaster.ItemId;
                        await RcptDetailService.AddRcptDetail(rcpt);
                        rcpt = new RcptDetail();
                    }
                    else
                    {
                        WarningContentMessage = "Specified Item not found in the Item Master. Please add to the Master first...";
                        Warning.OpenDialog();
                    }
                }
                rcptvoudetails = await RcptDetailService.GetRcptDetails(vRcptvouId);
                //await RcptDetGrid.Refresh();
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
        private async Task OnFiltering(Syncfusion.Blazor.DropDowns.FilteringEventArgs args)
        {
            Custom = args.Text;
            args.PreventDefaultAction = true;
            var query = new Query().Where(new WhereFilter() { Field = "ItemDesc", Operator = "contains", value = args.Text, IgnoreCase = true });
            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();
            await ComboObj.FilterAsync(ItemMasterList, query);
        }
        private async Task CloseDialog()
        {
            await this.DialogAddManual.HideAsync();
        }
        private async Task OnFilteringBranch(Syncfusion.Blazor.DropDowns.FilteringEventArgs args)
        {
            Custom = args.Text;
            args.PreventDefaultAction = true;
            var query1 = new Query().Where(new WhereFilter() { Field = "BranchDesc", Operator = "contains", value = args.Text, IgnoreCase = true });
            query1 = !string.IsNullOrEmpty(args.Text) ? query1 : new Query();
            await Company.FilterAsync(companylist, query1);
        }
    }
}