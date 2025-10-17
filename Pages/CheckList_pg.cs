using DigiEquipSys.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using System.Text;

namespace DigiEquipSys.Pages
{
    public partial class CheckList_pg
    {

        [CascadingParameter]
        public EventCallback notify { get; set; }
        private string? myUser;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    myUser = await sessionStorage.GetItemAsync<string>("adminEmail");
                    await notify.InvokeAsync();
                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                    return;
                }
            }
        }
        private ItemMaster? im;

        [Parameter]
        public string? vEdit { get; set; }
        [Parameter] public long RcptvouId { get; set; }
        public class RcptDetailSumm
        {
            public string? RdScanCode { get; set; }
            public string? RdListNo { get; set; }
            public string? RdLotNo { get; set; }
            public DateTime RdExpiryDate { get; set; }
            public long RdStkIdDesc { get; set; }
            public string? RdStkIdUnit { get; set; }
            public decimal RdUp { get; set; }
            public decimal RdQty { get; set; }
            public decimal TotalPrice { get; set; }
            public string? RdSuppCode { get; set; }
            public string? RdPohDispNo { get; set; }
            public string? RdClientCode { get; set; }
            public string? RdGrp { get; set; }
            public string? RdCat { get; set; }
            public long RdId { get; set; }
            public string? RdInvNo { get; set; }
            public DateTime? RdInvDate { get; set; }
            public long RdStockStkId { get; set; }
            public List<PoHead> PohList { get; set; } = new();

        }

        private SfGrid<RcptDetailSumm>? RcptDetGridSumm;
        protected List<ItemMaster> ItemMasterList = new();
        protected List<RcptDetail> rcptvoudetails = new();
        protected List<RcptDetailSumm> rcptvoudetailsSumm = new();
        private RcptDetailSumm rcptaddedit = new();
        private RcptHead? rcpt = new();
        private RcptDetail rcptDet = new();
        //private SupplierMaster supp = new();
        private Stock stk = new();
        //private string? suppname { get; set; }
        private string? vRdListNo { get; set; }
        private string? vRdLotNo { get; set; }
        private DateTime vRdExpiryDate { get; set; }
        private bool isApprove = true;
        private bool isDisApprove = true;
        protected List<SupplierMaster> SupplierMasterList = new();
        protected List<ClientMaster> ClientMasterList = new();
        public List<ItemUnit>? ItemUnitList = new();
        public List<GroupMaster>? ItemGroupList = new();
        public List<CategMaster>? ItemCatList = new();
        private SfComboBox<string, ClientMaster> ComboObj;
        private SfComboBox<string, SupplierMaster> ComboObj2;
        private string Custom { get; set; }
        private bool showEditButton = false;
        private RcptDetailSumm selectedDetail;
        public bool IsVisRole { get; set; } = true;
        private string? myRole;
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
        protected List<PoDetail>povoudetails = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                myRole = await sessionStorage.GetItemAsync<string>("adminRo");
                SupplierMasterList = await mySupplierService.GetSuppliers();
                ClientMasterList = await myClientService.GetClients();
                rcpt = await RcptHeadService.GetRcptHead(RcptvouId);
                if (rcpt.RhNo != null)
                {
                    if (vEdit != "No")
                    {
                        showEditButton = true;
                        if (rcpt.RhApproved == true)
                        {
                            isApprove = true;
                            isDisApprove = false;
                            //if (myRole == "01")
                            //{
                            //    isDisApprove = false;
                            //}
                        }
                        else
                        {
                            isApprove = false;
                            isDisApprove = true;
                        }
                    }

                }

                if (myRole == "01" || myRole == "02" || myRole == "03")
                {
                    IsVisRole = true;
                }
                else
                {
                    IsVisRole = false;
                }


                ItemUnitList = await myUnitService.GetItemUnits();
                ItemMasterList = await myItemMaster.GetItemMasters();
                ItemGroupList = await myGrpService.GetGroupMasters();
                ItemCatList = await myCatService.GetCategMasters();

                rcptvoudetails = await RcptDetailService.GetRcptDetails(RcptvouId);
                var tt = rcptvoudetails
                      .GroupBy(r => new { r.RdScanCode, r.RdListNo, r.RdLotNo, r.RdExpiryDate, r.RdStkIdDesc, r.RdSuppCode, r.RdPohDispNo, r.RdClientCode, r.RdStkIdUnit, r.RdUp, r.RdStkIdGrp, r.RdStkIdCat,r.RdVendInvNo,r.RdVendInvDate,r.RdStockStkId })
                      .Select(g => new RcptDetailSumm
                      {
                          RdScanCode = g.Key.RdScanCode,
                          RdListNo = g.Key.RdListNo,
                          RdLotNo = g.Key.RdLotNo,
                          RdExpiryDate = Convert.ToDateTime(g.Key.RdExpiryDate),
                          RdStkIdDesc = Convert.ToInt64(g.Key.RdStkIdDesc),
                          RdStkIdUnit = g.Key.RdStkIdUnit,
                          RdClientCode = g.Key.RdClientCode,
                          RdSuppCode = g.Key.RdSuppCode,
                          RdPohDispNo = g.Key.RdPohDispNo,
                          RdUp = Convert.ToDecimal(g.Key.RdUp),
                          RdQty = g.Sum(r => Convert.ToDecimal(r.RdQty)),
                          TotalPrice = g.Sum(r => Convert.ToDecimal(r.TotalPrice)),
                          RdGrp = g.Key.RdStkIdGrp,
                          RdCat = g.Key.RdStkIdCat,
                          RdId = g.Min(r => Convert.ToInt64(r.RdId)),
                          RdInvNo = g.Key.RdVendInvNo,
                          RdInvDate = g.Key.RdVendInvDate ?? null,
                          RdStockStkId = g.Key.RdStockStkId ?? 0
                      }).ToList();
                rcptvoudetailsSumm = tt;
                TotalItems = rcptvoudetailsSumm.Count();
                TotalAmount = Math.Round(rcptvoudetailsSumm.Sum(d => d.TotalPrice), 2);
                RcptDetGridSumm?.Refresh();
                await InvokeAsync(StateHasChanged);
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
        public async Task Previous()
        {
            try
            {
                if (vEdit == "No")
                {
                    NavigationManager.NavigateTo($"rcptHead_pg/{RcptvouId}/");
                }
                else
                {
                    NavigationManager.NavigateTo($"rcptHeadEdit_pg/{RcptvouId}/");
                    // await Task.Delay(100);
                    //await JSRuntime.InvokeVoidAsync("goBack");
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        public async Task GrnEdit()
        {
            try
            {
                NavigationManager.NavigateTo($"rcptHeadEdit_pg/{RcptvouId}/");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        protected async Task DisApprove()
        {
            try
            {
                await StockService.ExecuteWithTransactionAsync(async () =>
                {
                    bool allStockUpdatesSucceeded = true;
                    bool anyStockUpdated = false;

                    if (rcptvoudetailsSumm != null)
                    {
                        foreach (var individualEntry in rcptvoudetailsSumm)
                        {
                            if (individualEntry.RdPohDispNo != null)
                            {
                                var myPohead = await myPoheadService.GetPoHeadByPoNumber(individualEntry.RdPohDispNo);
                                if (myPohead != null)
                                {
                                    var myPoDet = await myPoDetailService.GetPoDetail(myPohead.PohId, individualEntry.RdListNo);
                                    if (myPoDet != null)
                                    {
                                        myPoDet.PodRcvdQty = myPoDet.PodRcvdQty - individualEntry.RdQty;
                                        var podetupdate = await myPoDetailService.UpdatePoDetail(myPoDet);
                                        if (podetupdate != "Success")  // You must make UpdateStock return bool
                                        {
                                            allStockUpdatesSucceeded = false;
                                            throw new Exception("PoDetail update failed for List No: " + individualEntry.RdListNo);
                                        }
                                    }
                                }
                            }

                            //check if at all delivery against this stockId
                            // you have this stock delivered in del_disp Number... disapprove this before this
                            //check if at all Journal against this stockId
                            // you have this stock in JvDet... disapprove this before this  

                            var chkStkId=individualEntry.RdStockStkId;

                            var vSaleStock = await myVwSaleService.GetvwSalesStockList(chkStkId);
                            var sb = new StringBuilder();
                            decimal mqty=0.00M;
                            decimal vRdQty = individualEntry.RdQty;
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
                            if (sb.Length >0)
                            {
                                allStockUpdatesSucceeded = false;
                                throw new InvalidOperationException("You can't Disapprove this Receipt. Already an Outward... " + sb);
                            }

                            if (dsw == 0)
                            {
                                vRdQty = vRdQty - mqty;
                                var vTrfStock = await myVwTransferService.GetvwTransfersStockList(chkStkId);
                                var sb1 = new StringBuilder();
                                decimal Trqty = 0.00M;
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
                                    throw new InvalidOperationException("You can't Disapprove this Receipt.Already a Journal... " + sb1);
                                }
                            }
                            var myStk = await StockService.GetStock(individualEntry.RdListNo, individualEntry.RdLotNo, individualEntry.RdExpiryDate, individualEntry.RdClientCode);
                            if (myStk == null)
                            {
                                throw new Exception("Item not found in stock..Critical Error...");
                            }
                            var expectedPurQty = (myStk.ItemPurQty ?? 0) - individualEntry.RdQty;
                            if (myStk.ItemPurQty == expectedPurQty) // Only update if not already updated
                            {
                                // Stock already updated once → fail fast
                                throw new Exception($"Stock for List No {individualEntry.RdListNo} " +
                                                    $"Lot No {individualEntry.RdLotNo} already updated. " +
                                                    $"Approval stopped to avoid duplicate processing.");
                            }
                            myStk.ItemPurQty = myStk.ItemPurQty - individualEntry.RdQty;
                            myStk.ItemPurAmt = myStk.ItemPurAmt - (individualEntry.RdQty * individualEntry.RdUp);
                            var updated = await StockService.UpdateStock(myStk);
                            if (updated != "Success")  // You must make UpdateStock return bool
                            {
                                allStockUpdatesSucceeded = false;
                                throw new Exception("Stock update failed for List No: " + individualEntry.RdListNo);
                            }
                            anyStockUpdated = true;
                        }
                        if (allStockUpdatesSucceeded && anyStockUpdated && rcpt != null)
                        {
                            rcpt.RhApproved = false;
                            var rcptHeadUpdated = await RcptHeadService.UpdateRcptHead(rcpt);
                            if (rcptHeadUpdated != "Success")
                            {
                                throw new Exception("RcptHead update failed.");
                            }
                            isApprove = false;
                            isDisApprove = true;
                            string message = "Disapproved Successfully...";
                            await JSRuntime.InvokeVoidAsync("alert", message);
                        }
                        else
                        {
                            throw new Exception("Update failed.. Try Again !!");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        protected async Task Approve()
        {
            try
            {
                await StockService.ExecuteWithTransactionAsync(async () =>
                {
                    bool allStockUpdatesSucceeded = true;
                    bool anyStockUpdated = false;
                    if (rcptvoudetailsSumm != null)
                    {
                        foreach (var individualEntry in rcptvoudetailsSumm)
                        {

                            if (individualEntry.RdPohDispNo != null)
                            {
                                var myPohead = await myPoheadService.GetPoHeadByPoNumber(individualEntry.RdPohDispNo);
                                if (myPohead != null)
                                {
                                    var myPoDet = await myPoDetailService.GetPoDetail(myPohead.PohId, individualEntry.RdListNo);
                                    if (myPoDet !=null)
                                    {
                                        myPoDet.PodRcvdQty = (myPoDet.PodRcvdQty ?? 0) + individualEntry.RdQty;
                                        var podetupdate = await myPoDetailService.UpdatePoDetail(myPoDet);
                                        if (podetupdate != "Success")  // You must make UpdateStock return bool
                                        {
                                            allStockUpdatesSucceeded = false;
                                            throw new Exception("PoDetail update failed for List No: " + individualEntry.RdListNo);
                                        }
                                    }
                                }
                            }

                            if (individualEntry.RdInvDate == null || individualEntry.RdInvNo == null || individualEntry.RdClientCode == null)
                            {
                                //await JSRuntime.InvokeVoidAsync("alert", "Invalid Entry for Vendor Invoice Date or Vendor Invoice Number.");
                                throw new InvalidOperationException("Invalid Entry for Vendor Invoice Date or Vendor Invoice Number.");
                            }
                            var myStk = await StockService.GetStockDescending(individualEntry.RdListNo, individualEntry.RdLotNo, individualEntry.RdExpiryDate, individualEntry.RdClientCode);
                            if (myStk == null)
                            {
                                stk.ItemScanCode = individualEntry.RdScanCode;
                                stk.ItemListNo = individualEntry.RdListNo;
                                stk.ItemLotNo = individualEntry.RdLotNo;
                                stk.ItemExpiryDate = individualEntry.RdExpiryDate;
                                stk.ItemClientCode = individualEntry.RdClientCode;
                                stk.ItemPurQty = individualEntry.RdQty;
                                stk.ItemUp = individualEntry.RdUp;
                                stk.ItemPurAmt = individualEntry.RdUp * individualEntry.RdQty;
                                stk.ItemStkIdGrp = individualEntry.RdGrp;
                                stk.ItemStkIdCat = individualEntry.RdCat;
                                stk.ItemSuppCode = individualEntry.RdSuppCode;
                                stk.ItemStkIdUnit = individualEntry.RdStkIdUnit;
                                stk.ItemBatchId = 1;

                                var myIt = await myItemMaster.GetItemMaster(individualEntry.RdListNo, individualEntry.RdClientCode);
                                if (myIt != null)
                                {
                                    stk.ItemId = myIt.ItemId;
                                    stk.ItemStkIdDesc = myIt.ItemId;
                                    stk.ItemCp = myIt.ItemCostPrice;
                                    stk.ItemSp = myIt.ItemSellPrice;
                                }
                                else
                                {
                                    await JSRuntime.InvokeVoidAsync("alert", "No Item Master Found for the specified Client for the Item " + individualEntry.RdListNo);
                                    throw new InvalidOperationException("Item Master not found.");
                                }

                                var createdStock = await StockService.CreateStock(stk);
                                if (createdStock == null || createdStock.StkId <= 0)
                                {
                                    allStockUpdatesSucceeded = false;
                                    throw new InvalidOperationException("Stock not created.");
                                }

                                rcptDet=await RcptDetailService.GetRcptDetail(individualEntry.RdId);
                                if (rcptDet == null)
                                {
                                    allStockUpdatesSucceeded = false;
                                    throw new InvalidOperationException("Rcpt Detail not found.");
                                }
                                else
                                {
                                    rcptDet.RdStockStkId = createdStock.StkId;
                                    var rcptDetUpdated = await RcptDetailService.UpdateRcptDetail(rcptDet);
                                    if (rcptDetUpdated != "Success")
                                    {
                                        allStockUpdatesSucceeded = false;
                                        throw new Exception("Rcpt Detail update failed.");
                                    }
                                }
                                stk = new Stock();
                            }
                            else
                            {
                                var myIt = await myItemMaster.GetItemMaster(individualEntry.RdListNo, individualEntry.RdClientCode);
                                if (myIt != null)
                                {
                                    stk.ItemId = myIt.ItemId;
                                    stk.ItemStkIdDesc = myIt.ItemId;
                                    stk.ItemCp = myIt.ItemCostPrice;
                                    stk.ItemSp = myIt.ItemSellPrice;

                                }
                                else
                                {
                                    await JSRuntime.InvokeVoidAsync("alert", "No Item Master Found for the specified Client for the Item " + individualEntry.RdListNo);
                                    throw new InvalidOperationException("Item Master not found.");
                                }

                                if (myIt.ItemCostPrice == myStk.ItemCp)
                                {
                                    var expectedPurQty = (myStk.ItemPurQty ?? 0) + individualEntry.RdQty;
                                    if (myStk.ItemPurQty == expectedPurQty) // Only update if not already updated
                                    {
                                        // Stock already updated once → fail fast
                                        throw new Exception($"Stock for List No {individualEntry.RdListNo} " +
                                                            $"Lot No {individualEntry.RdLotNo} already updated. " +
                                                            $"Approval stopped to avoid duplicate processing.");
                                    }
                                    myStk.ItemPurQty = (myStk.ItemPurQty ?? 0) + individualEntry.RdQty;
                                    myStk.ItemPurAmt = (myStk.ItemPurAmt ?? 0) + (individualEntry.RdQty * individualEntry.RdUp);
                                    var updated = await StockService.UpdateStock(myStk);
                                    if (updated != "Success")  // You must make UpdateStock return bool
                                    {
                                        allStockUpdatesSucceeded = false;
                                        throw new Exception("Stock update failed for List No: " + individualEntry.RdListNo);
                                    }
                                    rcptDet = await RcptDetailService.GetRcptDetail(individualEntry.RdId);
                                    if (rcptDet == null)
                                    {
                                        allStockUpdatesSucceeded = false;
                                        throw new InvalidOperationException("Rcpt Detail not found.");
                                    }
                                    else
                                    {
                                        rcptDet.RdStockStkId = stk.StkId;
                                        var rcptDetUpdated = await RcptDetailService.UpdateRcptDetail(rcptDet);
                                        if (rcptDetUpdated != "Success")
                                        {
                                            allStockUpdatesSucceeded = false;
                                            throw new Exception("Rcpt Detail update failed.");
                                        }
                                    }
                                }
                                else
                                {
                                    stk.ItemScanCode = individualEntry.RdScanCode;
                                    stk.ItemListNo = individualEntry.RdListNo;
                                    stk.ItemLotNo = individualEntry.RdLotNo;
                                    stk.ItemExpiryDate = individualEntry.RdExpiryDate;
                                    stk.ItemClientCode = individualEntry.RdClientCode;
                                    stk.ItemPurQty = individualEntry.RdQty;
                                    stk.ItemCp = myIt.ItemCostPrice;
                                    stk.ItemSp = myIt.ItemSellPrice;
                                    stk.ItemUp = individualEntry.RdUp;
                                    stk.ItemPurAmt = myIt.ItemCostPrice * individualEntry.RdQty;
                                    stk.ItemStkIdGrp = individualEntry.RdGrp;
                                    stk.ItemStkIdCat = individualEntry.RdCat;
                                    stk.ItemSuppCode = individualEntry.RdSuppCode;
                                    stk.ItemStkIdUnit = individualEntry.RdStkIdUnit;
                                    stk.ItemId = myIt.ItemId;
                                    stk.ItemStkIdDesc = myIt.ItemId;
                                    stk.ItemBatchId = myStk.ItemBatchId + 1;

                                    var createdStock = await StockService.CreateStock(stk);
                                    if (createdStock == null || createdStock.StkId <= 0)
                                    {
                                        allStockUpdatesSucceeded = false;
                                        throw new InvalidOperationException("Stock not created.");
                                    }
                                    rcptDet = await RcptDetailService.GetRcptDetail(individualEntry.RdId);
                                    if (rcptDet == null)
                                    {
                                        allStockUpdatesSucceeded = false;
                                        throw new InvalidOperationException("Rcpt Detail not found.");
                                    }
                                    else
                                    {
                                        rcptDet.RdStockStkId = createdStock.StkId;
                                        var rcptDetUpdated = await RcptDetailService.UpdateRcptDetail(rcptDet);
                                        if (rcptDetUpdated != "Success")
                                        {
                                            allStockUpdatesSucceeded = false;
                                            throw new Exception("Rcpt Detail update failed.");
                                        }
                                    }
                                    stk = new Stock();
                                }
                            }
                            anyStockUpdated = true;
                        }
                        if (allStockUpdatesSucceeded && anyStockUpdated && rcpt != null)
                        {
                            rcpt.RhApproved = true;
                            var rcptHeadUpdated = await RcptHeadService.UpdateRcptHead(rcpt);
                            if (rcptHeadUpdated != "Success")
                            {
                                throw new Exception("RcptHead update failed.");
                            }
                            isApprove = true;
                            isDisApprove = true;
                            if (myRole == "01")
                            {
                                isDisApprove = false;
                            }

                            string message = "Approved Successfully...";
                            await JSRuntime.InvokeVoidAsync("alert", message);
                        }
                        else
                        {
                            throw new Exception("Update failed.. Try Again !!");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
            }
        }
        public async Task RowUpdatingHandler(RowUpdatingEventArgs<RcptDetailSumm> args)
        {
            try
            {
                if (args.Data.RdClientCode != null && args.Data.RdSuppCode != null && args.Data.RdInvNo != null && args.Data.RdInvDate != null)
                {
                    if (@isApprove == false)
                    {
                        im = await myItemMaster.GetItemMaster(args.Data.RdListNo, args.Data.RdClientCode);
                        if (im != null)
                        {
                            args.Data.RdUp = Convert.ToDecimal(im.ItemCostPrice);
                            args.Data.RdGrp = im.ItemGrpCode;
                            args.Data.RdCat = im.ItemCatCode;
                        }
                        else
                        {
                            args.Cancel = true;
                            await JSRuntime.InvokeVoidAsync("alert", "Item is not in the Customer List..");
                            return;
                        }

                        int i = 1;
                        var qry = (from x in rcptvoudetails where x.RdId == args.Data.RdId && x.RdListNo == args.Data.RdListNo && x.RdLotNo == args.Data.RdLotNo && x.RdExpiryDate == args.Data.RdExpiryDate select x).FirstOrDefault();
                        if (qry != null)
                        {
                            var qry1 = (from y in rcptvoudetails where y.RdRhId == qry.RdRhId && y.RdListNo == qry.RdListNo && y.RdLotNo == qry.RdLotNo && y.RdExpiryDate == qry.RdExpiryDate select y).ToList();

                            if (qry.RdQty > args.Data.RdQty)
                            {

                                rcptDet.RdQty = (qry.RdQty ?? 0) - (args.Data.RdQty);
                                qry.RdQty = args.Data.RdQty;
                                await RcptDetailService.UpdateRcptDetail(qry);


                                rcptDet.RdId = 0;
                                rcptDet.RdRhId = qry.RdRhId;
                                rcptDet.RdListNo = qry.RdListNo;
                                rcptDet.RdLotNo = qry.RdLotNo;
                                rcptDet.RdExpiryDate = qry.RdExpiryDate;
                                rcptDet.RdUp = 0;
                                rcptDet.RdStkIdDesc = qry.RdStkIdDesc;
                                rcptDet.RdStkIdGrp = qry.RdStkIdGrp;
                                rcptDet.RdStkIdCat = qry.RdStkIdCat;
                                rcptDet.RdStkIdUnit = qry.RdStkIdUnit;
                                rcptDet.RdStkId = qry.RdStkId;
                                await RcptDetailService.AddRcptDetail(rcptDet);
                                rcptDet = new RcptDetail();

                                rcptvoudetails = await RcptDetailService.GetRcptDetails(RcptvouId);
                                qry1 = (from y in rcptvoudetails where y.RdRhId == qry.RdRhId && y.RdListNo == qry.RdListNo && y.RdLotNo == qry.RdLotNo && y.RdExpiryDate == qry.RdExpiryDate select y).ToList();
                                i = Convert.ToInt32(args.Data.RdQty);
                            }

                            foreach (var q in qry1)
                            {
                                if (i <= args.Data.RdQty)
                                {
                                    if (q.RdClientCode == null || q.RdSuppCode == null || q.RdVendInvNo == null || q.RdVendInvDate == null)  //|| q.RdPohDispNo == null TO BE INCLUDED LATER WHEN PURCHASE ORDER IS READY
									{
                                        q.RdClientCode = args.Data.RdClientCode;
                                        q.RdSuppCode = args.Data.RdSuppCode;
                                        q.RdVendInvNo = args.Data.RdInvNo;
                                        q.RdVendInvDate = args.Data.RdInvDate;
                                        q.RdUp = args.Data.RdUp;
                                        q.RdStkIdGrp = args.Data.RdGrp;
                                        q.RdStkIdCat = args.Data.RdCat;
                                        q.RdPohDispNo = args.Data.RdPohDispNo;
                                        await RcptDetailService.UpdateRcptDetail(q);
                                        i++;
                                    }
                                }
                            }
                            rcptvoudetails = await RcptDetailService.GetRcptDetails(RcptvouId);
                            var tt = rcptvoudetails
                                  .GroupBy(r => new { r.RdScanCode, r.RdListNo, r.RdLotNo, r.RdExpiryDate, r.RdStkIdDesc, r.RdSuppCode, r.RdClientCode, r.RdStkIdUnit, r.RdUp, r.RdStkIdGrp, r.RdStkIdCat, r.RdVendInvDate, r.RdVendInvNo,r.RdPohDispNo, r.TotalPrice })
                                  .Select(g => new RcptDetailSumm
                                  {
                                      RdScanCode = g.Key.RdScanCode,
                                      RdListNo = g.Key.RdListNo,
                                      RdLotNo = g.Key.RdLotNo,
                                      RdExpiryDate = Convert.ToDateTime(g.Key.RdExpiryDate),
                                      RdStkIdDesc = Convert.ToInt64(g.Key.RdStkIdDesc),
                                      RdStkIdUnit = g.Key.RdStkIdUnit,
                                      RdClientCode = g.Key.RdClientCode,
                                      RdSuppCode = g.Key.RdSuppCode,
                                      RdUp = Convert.ToDecimal(g.Key.RdUp),
                                      TotalPrice = Convert.ToDecimal(g.Key.TotalPrice),
                                      RdQty = g.Sum(r => Convert.ToDecimal(r.RdQty)),
                                      RdGrp = g.Key.RdStkIdGrp,
                                      RdCat = g.Key.RdStkIdCat,
                                      RdId = g.Min(r => Convert.ToInt64(r.RdId)),
                                      RdInvNo = g.Key.RdVendInvNo,
                                      RdInvDate = g.Key.RdVendInvDate ?? null,
                                      RdPohDispNo = g.Key.RdPohDispNo  
                                  }).ToList();
                            rcptvoudetailsSumm = tt;
                            TotalItems = rcptvoudetailsSumm.Count();
                            TotalAmount = Math.Round(rcptvoudetailsSumm.Sum(d => d.TotalPrice), 2);
                            RcptDetGridSumm?.Refresh();
                            StateHasChanged();

                            //StateHasChanged();
                            //await RcptDetGridSumm.Refresh();

                        }
                        else
                        {
                            args.Cancel = true;
                            await JSRuntime.InvokeVoidAsync("alert", "Approved Voucher..You Can't Add/Edit..");
                            return;
                        }
                        //NavigationManager.NavigateTo($"/test");
                        //await Task.Delay(200);
                        //NavigationManager.NavigateTo($"/checkList_pg/" + RcptvouId + "/Yes", forceLoad: true);
                        NavigationManager.NavigateTo($"/checkList_pg/" + RcptvouId + "/Yes");
                    }
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
        private async Task OnFiltering(Syncfusion.Blazor.DropDowns.FilteringEventArgs args)
        {
            Custom = args.Text;
            args.PreventDefaultAction = true;
            var query = new Query().Where(new WhereFilter() { Field = "ClientName", Operator = "contains", value = args.Text, IgnoreCase = true });
            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();
            await ComboObj.FilterAsync(ClientMasterList, query);
        }
        private async Task OnFiltering2(Syncfusion.Blazor.DropDowns.FilteringEventArgs args)
        {
            Custom = args.Text;
            args.PreventDefaultAction = true;
            var query = new Query().Where(new WhereFilter() { Field = "SuppName", Operator = "contains", value = args.Text, IgnoreCase = true });
            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();
            await ComboObj2.FilterAsync(SupplierMasterList, query);
        }

        //private async Task UpdatePurchaseOrderDropdown(RcptDetailSumm order)
        //{

        //    if (!string.IsNullOrEmpty(order?.RdSuppCode))
        //    {
        //        var result = await myPoheadService.GetPoHeads(order.RdSuppCode);
        //        order.PohList = result ?? new List<PoHead>();
        //    }
        //    else
        //    {
        //        order.PohList = new();
        //    }
        //    await Task.Delay(100);
        //    await InvokeAsync(StateHasChanged);
        //    await RcptDetGridSumm.Refresh();
        //}

        private void OnRowSelected(RowSelectEventArgs<RcptDetailSumm> args)
        {
            selectedDetail = args.Data;
        }
        public async Task RowDeletingHandler(RowDeletingEventArgs<RcptDetailSumm> args)
        {
            try
            {
                var det = selectedDetail;
                var myQry = (from w in rcptvoudetailsSumm where w.RdId == det.RdId select w).FirstOrDefault();
                if (myQry != null)
                {
                    if (det.RdListNo != null)
                    {
                        if (@isApprove == false)
                        {
                            int i = 1;
                            var qry = (from x in rcptvoudetails where x.RdId == det.RdId && x.RdListNo == det.RdListNo && x.RdLotNo == det.RdLotNo && x.RdExpiryDate == det.RdExpiryDate select x).FirstOrDefault();
                            if (qry != null)
                            {
                                var qry1 = (from y in rcptvoudetails where y.RdRhId == qry.RdRhId select y).ToList();
                                foreach (var q in qry1)
                                {
                                    if (i <= det.RdQty)
                                    {
                                        if (q.RdClientCode == det.RdClientCode && q.RdListNo == det.RdListNo && q.RdLotNo == det.RdLotNo && q.RdExpiryDate == det.RdExpiryDate)
                                        {
                                            q.RdStkIdGrp = null;
                                            q.RdStkIdCat = null;
                                            q.RdClientCode = null;
                                            q.RdSuppCode = null;
                                            q.RdPohDispNo = null;
                                            q.RdVendInvNo = null;
                                            q.RdVendInvDate = null;
                                            q.RdUp = 0;
                                            await RcptDetailService.UpdateRcptDetail(q);
                                            i++;
                                        }
                                    }
                                }
                                rcptvoudetails = await RcptDetailService.GetRcptDetails(RcptvouId);
                                var tt = rcptvoudetails
                                      .GroupBy(r => new { r.RdScanCode, r.RdListNo, r.RdLotNo, r.RdExpiryDate, r.RdStkIdDesc, r.RdSuppCode, r.RdPohDispNo, r.RdClientCode, r.RdStkIdUnit, r.RdUp, r.RdStkIdGrp, r.RdStkIdCat, r.RdVendInvDate, r.RdVendInvNo, r.TotalPrice })
                                      .Select(g => new RcptDetailSumm
                                      {
                                          RdScanCode = g.Key.RdScanCode,
                                          RdListNo = g.Key.RdListNo,
                                          RdLotNo = g.Key.RdLotNo,
                                          RdExpiryDate = Convert.ToDateTime(g.Key.RdExpiryDate),
                                          RdStkIdDesc = Convert.ToInt64(g.Key.RdStkIdDesc),
                                          RdStkIdUnit = g.Key.RdStkIdUnit,
                                          RdClientCode = g.Key.RdClientCode,
                                          RdSuppCode = g.Key.RdSuppCode,
                                          RdPohDispNo = g.Key.RdPohDispNo,
                                          RdUp = Convert.ToDecimal(g.Key.RdUp),
                                          RdQty = g.Sum(r => Convert.ToDecimal(r.RdQty)),
                                          TotalPrice = Convert.ToDecimal(g.Key.TotalPrice),
                                          RdGrp = g.Key.RdStkIdGrp,
                                          RdCat = g.Key.RdStkIdCat,
                                          RdId = g.Min(r => Convert.ToInt64(r.RdId)),
                                          RdInvNo = g.Key.RdVendInvNo,
                                          RdInvDate = g.Key.RdVendInvDate ?? null
                                      }).ToList();
                                rcptvoudetailsSumm = tt;
                                TotalItems = rcptvoudetailsSumm.Count();
                                TotalAmount = Math.Round(rcptvoudetailsSumm.Sum(d => d.TotalPrice), 2);
                                RcptDetGridSumm?.Refresh();
                                StateHasChanged();
                            }
                            else
                            {
                                args.Cancel = true;
                                await JSRuntime.InvokeVoidAsync("alert", "Approved Voucher..You Can't Add/Edit..");
                                return;
                            }
                            NavigationManager.NavigateTo($"/checkList_pg/" + RcptvouId + "/Yes");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
    }
}
