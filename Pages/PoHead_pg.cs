using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using DigiEquipSys.Services;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Navigations;

namespace DigiEquipSys.Pages
{
    public partial class PoHead_pg
    {
        private string? myUser;
        private string? myRole;
        private string? myLoc;
        public string? myLocShort { get; set; }

        [CascadingParameter]
        public EventCallback notify { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await InvokeAsync(StateHasChanged);
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

        private long tmpLineID { get; set; } = 0;
        public bool SpinnerVisible { get; set; } = false;
        public long PoSerNo { get; set; }
        public long? vPodId { get; set; }
        public long vPovouId { get; set; }
        public long vStkId { get; set; }
        private SfGrid<PoDetail>? PoDetGrid;

        [Parameter] public long PovouId { get; set; }

        public PoHead povouaddedit = new PoHead();

        [Inject]
        public IGenCurrencyService? GenCurrencyService { get; set; }
        public IEnumerable<GenCurrency>? currList;

        [Inject]
        public IPoHeadService? PoHeadService { get; set; }
        public IEnumerable<PoHead>? PoVouList;
        [Inject]
        public IPoDetailService? PoDetailService { get; set; }
        protected List<PoDetail> povoudetails = new();
        [Inject]
        public ISupplierService? SupplierService { get; set; }
        public IEnumerable<SupplierMaster>? suppList;
        private SupplierMaster povousupplier = new();

        public List<GroupMaster>? ItemGroupList = new();
        protected List<ItemMaster> ItemMasterList = new();
        protected List<ItemMaster> ItemMasterListDistinct = new();
        protected List<ClientMaster>? clientList = new();
        public List<CategMaster>? ItemCatList = new();
        public List<ItemUnit>? ItemUnitList = new();
        protected List<Branch> companylist = new();
        protected List<Division> branchlist = new();

        //public string Btn { get; set; }
        public PoDetail podetaddedit = new PoDetail();

        public string? GroupDesc { get; set; }
        public string? UnitDesc { get; set; }
        public long? PoNo { get; set; }

        private bool isButtonDisabled = false;
        private bool vSave = false;
        private bool isZoho = false;
        private bool vEnabled = true;

        public int iSw = 0;
        private SfComboBox<long?, ItemMaster> ComboObj;
        private SfComboBox<int?, SupplierMaster> ComboObj1;
        private SfComboBox<int?, ClientMaster> ComboObj2;
        private SfComboBox<string?, Branch> Company;

        private string Custom { get; set; }
        private bool isProgrammaticEdit = false;
        public bool TriggerClientPopup { get; set; }
        public string clcode { get; set; }
        private List<Object> ToolbarItems { get; set; }
        private string resultMessage = "";
        private decimal SubTotal;
        private decimal DiscTotal;
        private decimal TaxTotal;
        private decimal AdjAmount;
        private decimal GrandTotal;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                myUser = await sessionStorage.GetItemAsync<string>("adminEmail");
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                //myRole = await sessionStorage.GetItemAsync<string>("adminRo");

                this.SpinnerVisible = true;
                currList = await GenCurrencyService.GetCurrencies();
                povouaddedit.PohDate = DateTime.Now;
                povouaddedit.PohUser = myUser;
                povouaddedit.PohDateAltered = DateTime.Now;
				povouaddedit.PohCurr = "INR";
                povouaddedit.PohConvRate = 1.00M;
				suppList = await SupplierService.GetSuppliers();
                clientList = await myClientService.GetClients();


                if (PovouId != 0)
                {
                    vPovouId = PovouId;
                    povouaddedit = await PoHeadService.GetPoHead(PovouId);
                    PoSerNo = (long)povouaddedit.PohId;
                    povoudetails = await PoDetailService.GetPoDetails(PoSerNo);
                    isButtonDisabled = false;
                    vSave = false;
                    vEnabled = false;
                    povousupplier = await SupplierService.GetSupplier((int)povouaddedit.PohVendId);
                    SubTotal = povoudetails.Sum(x => (x.PodQty ?? 0.00M) * (x.PodUp ?? 0.00M));
                    DiscTotal = povoudetails.Sum(x => x.PodDiscAmt ?? 0.00M);
                    TaxTotal = povoudetails.Sum(x => x.PodGstAmt ?? 0.00M);
                    GrandTotal = povoudetails.Sum(x => x.PodAmount ?? 0.00M) + (povouaddedit.PohAdjustment ?? 0.00M);
                }
                else
                {
                    povouaddedit.PohComp = "01";   // Default company code
                    povouaddedit.PohBranch = "001";   // Default branch code
                }
                ItemMasterList = await myItemMaster.GetItemMasters();
                ItemGroupList = await myGroupService.GetGroupMasters();
                ItemCatList = await myCatService.GetCategMasters();
                ItemUnitList = await myUnitService.GetItemUnits();
                companylist = await myBranchService.GetBranches();
                branchlist = await myDivisionService.GetDivisions();

                ToolbarItems = new List<object>()
                {
                    "Add", "Delete", "Edit", "Cancel", "Update","Print",
                    new ItemModel() { Text = "ZohoUpdate", TooltipText = "Create P.O. to Zoho ", PrefixIcon = "e-export" }
                };


                await InvokeAsync(StateHasChanged);
                this.SpinnerVisible = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        private void ConnectToZoho()
        {
            var returnUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            var url = ZohoTokenService.GetAuthUrl(returnUrl);
            NavigationManager.NavigateTo(url, true);
        }
        public async Task CreateHandler()
        {
            await ComboObj.ShowPopupAsync();
        }

        private async Task OnFiltering(Syncfusion.Blazor.DropDowns.FilteringEventArgs args)
        {
            Custom = args.Text;
            args.PreventDefaultAction = true;
            var query = new Query().Where(new WhereFilter() { Field = "ItemDesc", Operator = "contains", value = args.Text, IgnoreCase = true });
            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();
            await ComboObj.FilterAsync(ItemMasterListDistinct, query);
        }

        private async Task OnFiltering1(Syncfusion.Blazor.DropDowns.FilteringEventArgs args)
        {
            Custom = args.Text;
            args.PreventDefaultAction = true;
            var query = new Query().Where(new WhereFilter() { Field = "SuppName", Operator = "contains", value = args.Text, IgnoreCase = true });
            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();
            await ComboObj1.FilterAsync(suppList, query);
        }
        private async Task OnFiltering2(Syncfusion.Blazor.DropDowns.FilteringEventArgs args)
        {
            Custom = args.Text;
            args.PreventDefaultAction = true;
            var query = new Query().Where(new WhereFilter() { Field = "ClientName", Operator = "contains", value = args.Text, IgnoreCase = true });
            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();
            await ComboObj2.FilterAsync(clientList, query);
        }
        private async Task mybranch(ChangeEventArgs<string?, Branch> args)
        {
            povouaddedit.PohBranch = "";
            branchlist = await myDivisionService.GetDivisions();
            var qbranchlist = branchlist.Where(c => c.LocBranchCode == args.Value).ToList();
            branchlist = qbranchlist.ToList();
        }
        protected async Task BatchSaveHandler(BeforeBatchSaveArgs<PoDetail> args)
        {
            iSw = 0;
            await PoDetGrid.Refresh();
        }

        protected void ActionCompleteHandler(ActionEventArgs<PoDetail> Args)
        {
            if (iSw == 0)
            {
                iSw = 1;
                povoudetails = povoudetails.Where(podet => !IsRowEmpty(podet)).ToList();
            }
        }
        public IEditorSettings myEditParams = new NumericEditCellParams
        {
            Params = new NumericTextBoxModel<object>() { ShowClearButton = true, ShowSpinButton = false }
        };
        private bool IsRowEmpty(PoDetail podet)
        {
            return string.IsNullOrEmpty(podet.PodListNo) || podet.PodStkIdDesc == null;
        }

        public void NoToFutureDate(ChangedEventArgs<DateTime?> args)
        {
            if (args.Value > DateTime.Today)
            {
                WarningHeaderMessage = "Invlaid Date!";
                WarningContentMessage = "Future Dates will not be accepted...";
                Warning.OpenDialog();
                povouaddedit.PohDate = DateTime.Now;
                return;
            }
        }
        public string GetTotalAggregate()
        {
            return Queryable.Sum(povoudetails.Select(x => (x.PodAmount)).AsQueryable()).ToString();
        }
        public async void CellSavedHandler(CellSavedArgs<PoDetail> args)
        {
            var podet = args.Data;
            switch (args.ColumnName)
            {
                case "PodStkIdDesc":
                    vEnabled = false;
                    var qrylistNo = ItemMasterListDistinct.FirstOrDefault(x => x.ItemId == podetaddedit.PodStkIdDesc);
                    if (qrylistNo != null)
                    {
                        podet.PodStkIdDesc = podetaddedit.PodStkIdDesc;
                        vStkId = qrylistNo.ItemId;
                        podet.PodListNo = qrylistNo.ItemListNo;
                        podet.PodUp=qrylistNo.ItemCostPrice;
                        podet.PodDiscPct = 0.00M;
                        podet.PodDiscAmt = 0.00M;
                        podet.PodStkIdGrp = qrylistNo.ItemGrpCode;
                        podet.PodStkIdCat = qrylistNo.ItemCatCode;
                        podet.PodStkIdUnit = qrylistNo.ItemUnit;
                        if (vPovouId == 0)
                        {
                            povouaddedit.PohId = 0;
                            var nextPohNo = await PoHeadService.GetNextPohNoAsync();
                            povouaddedit.PohNo = (long)nextPohNo;
                            povouaddedit.PohDispNo = "PO/" + myAccYear.myAccYear + "/" + nextPohNo.ToString().Trim();

                            await PoHeadService.CreatePoHead(povouaddedit);
                            PovouId = povouaddedit.PohId;
                            vPovouId = povouaddedit.PohId;
                            podet.PodId = 0;
                            podet.PodPohId = PovouId;
                            podet.PodStkIdDesc = vStkId;
                            var inserted = await PoDetailService.CreatePoDetail(podet);
                            vPodId = inserted?.PodId ?? 0;
                        }
                        else
                        {
                            if (vPodId == 0)
                            {
                                podet.PodId = 0;
                                podet.PodPohId = vPovouId;
                                podet.PodStkIdDesc = vStkId;
                                var inserted = await PoDetailService.CreatePoDetail(podet);
                                vPodId = inserted?.PodId ?? 0;
                            }
                            else
                            {
                                podet.PodId = (long)vPodId;
                                podet.PodStkIdDesc = vStkId;
                                await PoDetailService.UpdatePoDetail(podet); //var t1 =
                            }
                        }
                    }
                    else
                    {
                        string message = "No such Product Found ...Please add in the Item Master";
                        await JSRuntime.InvokeVoidAsync("alert", message);
                    }
                    break;


                case "PodQty":
                    podet.PodQty = args.Data.PodQty;
                    podet.PodAmount = ((args.Data.PodQty * args.Data.PodUp ?? 0.00M) - args.Data.PodDiscAmt ??0.00M) + (args.Data.PodGstAmt ?? 0.00M);
                    podet.PodId = (long)vPodId;
                    podet.PodStkIdDesc = vStkId;
                    podet.PodPohId = vPovouId;
                    if (args.Data.PodUp == 0.00M)
                    {
                        podet.PodUp = null;
                    }
                    await PoDetailService.UpdatePoDetail(podet); //var t1 =

                    //await Task.Delay(100);
                    //var rowIndex1 = await PoDetGrid.GetSelectedRowIndexesAsync();
                    //await PoDetGrid.EditCellAsync(rowIndex1.FirstOrDefault(), "PodUp");
                    break;

                case "PodUp":
                    podet.PodUp = args.Data.PodUp ?? 0.00M;
                    podet.PodAmount = ((args.Data.PodQty * args.Data.PodUp ?? 0.00M) - args.Data.PodDiscAmt ?? 0.00M) + (args.Data.PodGstAmt ?? 0.00M);
                    podet.PodId = (long)vPodId;
                    podet.PodStkIdDesc = vStkId;
                    podet.PodPohId = vPovouId;
                    if (args.Data.PodDiscAmt == 0.00M)
                    {
                        podet.PodDiscAmt = null;
                    }
                    await PoDetailService.UpdatePoDetail(podet);
                    //await Task.Delay(100);
                    //var rowIndex2 = await PoDetGrid.GetSelectedRowIndexesAsync();
                    //await PoDetGrid.EditCellAsync(rowIndex2.FirstOrDefault(), "PodDiscPct");
                    break;

                case "PodDiscAmt":
                    podet.PodAmount = (args.Data.PodQty * args.Data.PodUp ?? 0.00M) - args.Data.PodDiscAmt ?? 0.00M;
                    if (args.Data.PodUp == 0.00M)
                    {
                        podet.PodDiscPct = 0.00M;
                    }
                    else
                    {
                        podet.PodDiscPct = (args.Data.PodDiscAmt / (args.Data.PodQty * args.Data.PodUp ?? 0.00M)) * 100;
                    }
                    podet.PodId = (long)vPodId;
                    podet.PodStkIdDesc = vStkId;
                    podet.PodPohId = vPovouId;
                    podet.PodAmount = ((args.Data.PodQty * args.Data.PodUp ?? 0.00M) - args.Data.PodDiscAmt ?? 0.00M) + (args.Data.PodGstAmt ?? 0.00M);
                    await PoDetailService.UpdatePoDetail(podet); //var t1 =
                    break;

                case "PodDiscPct":
                    podet.PodDiscAmt = (args.Data.PodQty * args.Data.PodUp) * (args.Data.PodDiscPct / 100);
                    podet.PodAmount = (args.Data.PodQty * args.Data.PodUp) - ((args.Data.PodQty * args.Data.PodUp) * (args.Data.PodDiscPct / 100)) + (args.Data.PodGstAmt ?? 0.00M);
                    podet.PodId = (long)vPodId;
                    podet.PodStkIdDesc = vStkId;
                    podet.PodPohId = vPovouId;
                    await PoDetailService.UpdatePoDetail(podet); //var t1 =
                    break;

                case "PodGstPct":
                    podet.PodGstAmt = podet.PodAmount * (args.Data.PodGstPct / 100);
                    podet.PodAmount = (args.Data.PodQty * args.Data.PodUp) - ((args.Data.PodQty * args.Data.PodUp) * (args.Data.PodDiscPct / 100)) + (args.Data.PodGstAmt ?? 0.00M);
                    podet.PodId = (long)vPodId;
                    podet.PodStkIdDesc = vStkId;
                    podet.PodPohId = vPovouId;
                    await PoDetailService.UpdatePoDetail(podet); //var t1 =
                    break;

                default:
                    break;
            }
        }
        protected async Task PoVouSave()
        {
            PoDetGrid.EndEditAsync();
            PoDetGrid.Refresh();
            isButtonDisabled = false;
            this.SpinnerVisible = true;
            if (povouaddedit.PohVendId == 0)
            {
                WarningHeaderMessage = "Warning!";
                WarningContentMessage = "Please Select a Supplier before saving the order.";
                Warning.OpenDialog();
                return;
            }
            if (povouaddedit.PohComp == "" || povouaddedit.PohComp == null)
            {
                WarningHeaderMessage = "Warning!";
                WarningContentMessage = "Please Select a Company before saving the Delivery Note.";
                Warning.OpenDialog();
                return;
            }
            if (povouaddedit.PohBranch == "" || povouaddedit.PohBranch == null)
            {
                WarningHeaderMessage = "Warning!";
                WarningContentMessage = "Please Select a Branch before saving the Delivery Note.";
                Warning.OpenDialog();
                return;
            }
            if (vPovouId == 0)
            {
                PoVouList = await PoHeadService.GetPoHeads();
                var Qry = (from vou in PoVouList.OrderByDescending(x => x.PohNo) select vou).FirstOrDefault();
                if (Qry == null)
                {
                    povouaddedit.PohNo = 1000;
                }
                else
                {
                    povouaddedit.PohNo = Qry.PohNo + 1;
                }
                povouaddedit.PohDispNo = "PO/" + myLocShort + "/" + (Convert.ToString(povouaddedit.PohNo)).Trim();
                await PoHeadService.CreatePoHead(povouaddedit);
                PoSerNo = (long)povouaddedit.PohId;

                foreach (var individualEntry in povoudetails)
                {

                    individualEntry.PodId = 0;
                    individualEntry.PodPohId = PoSerNo;
                    try
                    {
                        await PoDetailService.CreatePoDetail(individualEntry);
                    }
                    catch (Exception ex)
                    {
                        throw ex.InnerException;
                    }
                }
                //NavigationManager.NavigateTo("po_pg");
            }
            else
            {
                await PoHeadService.UpdatePoHead(povouaddedit);
                foreach (var individualEntry in povoudetails)
                {
                    //If AccId is positive it means it has been edited during the edit of this voucher                
                    if (individualEntry.PodId > 0)
                    {
                        await PoDetailService.UpdatePoDetail(individualEntry); //var t1 =
                    }
                    else
                    //If AccId is negative it means it has been added during the edit of this voucher
                    {
                        individualEntry.PodId = 0;
                        individualEntry.PodPohId = PoSerNo;
                        await PoDetailService.CreatePoDetail(individualEntry); //var t1 =
                    }
                }
            }
            //await Task.Delay(1000);
            this.SpinnerVisible = false;
            string message = "Saved Successfully...";
            await JSRuntime.InvokeVoidAsync("alert", message);
        }
        public void RowSelectHandler(RowSelectEventArgs<PoDetail> args)
        {
            vPodId = args.Data.PodId;
            PovouId = Convert.ToInt64(args.Data.PodPohId);
        }

        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            try
            {
                if (args.Item.Text == "Add")
                {
                    if (povouaddedit.PohCustId == 0)
                    {
                        WarningHeaderMessage = "Invlaid Addition !";
                        WarningContentMessage = "You must select your customer!!!...";
                        Warning.OpenDialog();
                        args.Cancel = true;
                    }
                    if (povouaddedit.PohVendId == 0)
                    {
                        WarningHeaderMessage = "Invlaid Addition !";
                        WarningContentMessage = "You must select your Supplier!!!...";
                        Warning.OpenDialog();
                        args.Cancel = true;
                    }

                    if (isButtonDisabled == true)
                    {
                        WarningHeaderMessage = "Invlaid Addition !";
                        WarningContentMessage = "Approved Voucher... You can not Add!!!...";
                        Warning.OpenDialog();
                        args.Cancel = true;
                    }
                    if (povouaddedit.PohVendId != 0)
                    {
                        var qry1 = (from x in suppList where x.SuppId == povouaddedit.PohVendId select x).FirstOrDefault();
                        if (qry1 == null)
                        {
                            string message = "You haven't selected a supplier or not found...";
                            await JSRuntime.InvokeVoidAsync("alert", message);
                            args.Cancel = true;
                        }
                    }
                    if (povouaddedit.PohCustId != 0)
                    {
                        var qry1 = (from x in clientList where x.ClientId == povouaddedit.PohCustId select x).FirstOrDefault();
                        if (qry1 == null)
                        {
                            string message = "You haven't selected a Customer or Not found...";
                            await JSRuntime.InvokeVoidAsync("alert", message);
                            args.Cancel = true;
                        }
                        else
                        {
                            clcode = qry1.ClientCode;
                            ItemMasterListDistinct = await myItemMaster.GetItemMastersDistinct(clcode);
                        }
                    }
                }

                if (args.Item.Text == "Delete")
                {
                    args.Cancel = true;
                    if (vPodId > 0)
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

                if (args.Item.Text == "ZohoUpdate")
                {
                    var clname = clientList.FirstOrDefault(c => c.ClientId == povouaddedit.PohCustId).ClientName;

                    var token = await ZohoTokenService.GetAccessTokenAsync();
                    var orgId = await ZohoService.GetOrganizationIdAsync(token);

                    // var lineItems = povoudetails.Select(i => 
                    //     new {
                    //     item_id = "1169195000000768019",
                    //     units = i.PodStkIdUnit,
                    //     rate = (decimal)i.PodUp,
                    //     quantity = (decimal)i.PodQty
                    // }).ToList();

                    var lineItems = (
                        from p in povoudetails
                        join it in ItemMasterList on p.PodStkIdDesc equals it.ItemId
                        select new
                        {
                            item_id = it.ItemZohoItemId,
                            name=it.ItemListNo.Trim() + " - " + clname,
                            units = p.PodStkIdUnit,
                            rate = (decimal)p.PodUp,
                            quantity = (decimal)p.PodQty,
                            description = it.ItemDesc
                        }
                    ).ToList();

                    var currentInvoice = new
                    {
                        vendor_id = povousupplier.SuppZohoVendId,
                        purchaseorder_number = povouaddedit.PohDispNo,
                        reference_number = povouaddedit.PohVendRef,
                        date = povouaddedit.PohDate?.ToString("yyyy-MM-dd"),
                        source_of_supply = "TN",
                        destination_of_supply = "TN",
                        discount= DiscTotal,
                        //tax_type = "tax",
                        //tax_total=TaxTotal,
                        discount_account_id = "1169195000000000558",
                        exchange_rate = povouaddedit.PohConvRate,
                        line_items = lineItems
                    };

                    var payload = currentInvoice;
                    var result = await ZohoService.CreatePurchaseOrderAsync(token, orgId, payload);
                    var json = result; 
                    dynamic data = JsonConvert.DeserializeObject(json);
                    resultMessage = data.message;
                    await JSRuntime.InvokeVoidAsync("alert", resultMessage);

                }

            }
            catch (Exception ex)
            {
                resultMessage = $"Error: {ex.Message}";
                await JSRuntime.InvokeVoidAsync("alert", resultMessage);
            }
        }
        protected async Task ConfirmDelete(bool DeleteConfirmed)
        {
            try
            {
                this.SpinnerVisible = true;
                if (DeleteConfirmed)
                {
                    await PoDetailService.DeletePoDetail(Convert.ToInt64(vPodId));
                }
                await PoDetGrid.Refresh();
                povoudetails = await PoDetailService.GetPoDetails(PovouId);
                //TotalItems = povoudetails.Count;
                this.SpinnerVisible = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        public void Cancel()
        {
            NavigationManager.NavigateTo("po_pg/" + vCompType);
        }
        private async Task OnFilteringBranch(Syncfusion.Blazor.DropDowns.FilteringEventArgs args)
        {
            Custom = args.Text;
            args.PreventDefaultAction = true;
            var query1 = new Query().Where(new WhereFilter() { Field = "BranchDesc", Operator = "contains", value = args.Text, IgnoreCase = true });
            query1 = !string.IsNullOrEmpty(args.Text) ? query1 : new Query();
            await Company.FilterAsync(companylist, query1);
        }
        public async Task PoVouPrint()
        {
            PoNo = povouaddedit.PohNo;
            NavigationManager.NavigateTo("po_rep/" + PovouId + "/" + PoNo);
        }
         public void NavigateToPrevious()
        {
            NavigationManager.NavigateTo("po_pg/" + vCompType);
        }
        public async Task PoInvToZoho()
        {
            PoNo = povouaddedit.PohNo;
            NavigationManager.NavigateTo("pushInvoiceToZoho/" + PoNo);
        }
    }
}


//private async Task OnCombo1Blur(Microsoft.AspNetCore.Components.Web.FocusEventArgs args)
//{
//    // Small delay to ensure ComboObj2 is fully rendered
//    await Task.Delay(100);
//    if (ComboObj2 != null)
//    {
//        await ComboObj2.ShowPopupAsync();
//    }
//}


//var rowIndex2 = await PoDetGrid.GetSelectedRowIndexesAsync();
//int currentIndex = rowIndex2.FirstOrDefault();
//var currentData = await PoDetGrid.GetCurrentViewRecordsAsync();
//int dataCount = currentData.Count;
//if (currentIndex + 1 < dataCount)
//{
//    // Move to next existing row
//    await PoDetGrid.EditCellAsync(currentIndex + 1, "PodQty"); // or any first cell you prefer
//}
//else
//{
//    await PoDetGrid.AddRecordAsync(); // Adds a new row
//    await Task.Delay(100); // tweak this if needed
//    currentData = await PoDetGrid.GetCurrentViewRecordsAsync(); // refresh
//    int newIndex = currentData.Count - 1;
//    await PoDetGrid.EditCellAsync(newIndex, "PodStkIdDesc"); // or your default column
//}


//case "PodClientCode":
//    var qrylistNo1 = ItemMasterList.FirstOrDefault(x => x.ItemListNo == podet.PodListNo && x.ItemClientCode == podetaddedit.PodClientCode);
//    if (qrylistNo1 != null)
//    {
//        podet.PodListNo = qrylistNo1.ItemListNo;
//        podet.PodStkIdDesc = qrylistNo1.ItemId;
//        vStkId = qrylistNo1.ItemId;
//        podet.PodClientCode = podetaddedit.PodClientCode;
//        podet.PodDiscPct = 0.00M;
//        podet.PodDiscAmt = 0.00M;
//        podet.PodUp = 0.00M;
//        podet.PodStkIdGrp = qrylistNo1.ItemGrpCode;
//        podet.PodStkIdCat = qrylistNo1.ItemCatCode;
//        podet.PodStkIdUnit = qrylistNo1.ItemUnit;

//        if (povouaddedit.PohVendId == 0)
//        {
//            var qry1 = (from x in suppList where x.SuppId == povouaddedit.PohVendId select x).FirstOrDefault();
//            if (qry1 == null)
//            {
//                string message = "You haven't selected a supplier ...";
//                await JSRuntime.InvokeVoidAsync("alert", message);
//            }
//        }
//        if (vPovouId == 0)
//        {
//            povouaddedit.PohId = 0;
//            var nextPohNo = await PoHeadService.GetNextPohNoAsync();
//            povouaddedit.PohNo = (long)nextPohNo;
//            povouaddedit.PohDispNo = "PO/" + myAccYear.myAccYear + "/" + nextPohNo.ToString().Trim();
//            await PoHeadService.CreatePoHead(povouaddedit);
//            PovouId = povouaddedit.PohId;
//            vPovouId = povouaddedit.PohId;
//            podet.PodId = 0;
//            podet.PodPohId = PovouId;
//            podet.PodStkIdDesc = vStkId;
//            var inserted = await PoDetailService.CreatePoDetail(podet);
//            vPodId = inserted?.PodId ?? 0;
//        }
//        else
//        {
//            if (vPodId == 0)
//            {
//                podet.PodId = 0;
//                podet.PodPohId = vPovouId;
//                podet.PodStkIdDesc = vStkId;
//                var inserted = await PoDetailService.CreatePoDetail(podet);
//                vPodId = inserted?.PodId ?? 0;
//            }
//            else
//            {
//                podet.PodId = (long)vPodId;
//                podet.PodStkIdDesc = vStkId;
//                await PoDetailService.UpdatePoDetail(podet); //var t1 =
//            }
//        }
//        //var rowIndex = await PoDetGrid.GetSelectedRowIndexesAsync();
//        //await PoDetGrid.EditCellAsync(rowIndex.FirstOrDefault(), "PodQty");

//        await Task.Delay(100);
//        int rowIndex = povoudetails.FindIndex(x => x.PodId == vPodId);
//        if (rowIndex >= 0)
//        {
//            await PoDetGrid.SelectRowAsync(rowIndex);
//            await PoDetGrid.EditCellAsync(rowIndex, "PodQty");
//        }

//    }
//    else
//    {
//        string message = "No such Product Found for this Client ...Please add in the Item Master";
//        await JSRuntime.InvokeVoidAsync("alert", message);
//    }
//    break;



//if (povouaddedit.PohVendId == 0)
//{
//    var qry1 = (from x in suppList where x.SuppId == povouaddedit.PohVendId select x).FirstOrDefault();
//    if (qry1 == null)
//    {
//        string message = "You haven't selected a supplier ...";
//        await JSRuntime.InvokeVoidAsync("alert", message);
//    }
//}
//var rowIndex = await PoDetGrid.GetSelectedRowIndexesAsync();
//await PoDetGrid.EditCellAsync(rowIndex.FirstOrDefault(), "PodQty");