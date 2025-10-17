using DigiEquipSys.Models;
using DigiEquipSys.Shared;
using DigiEquipSys.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Grids.Internal;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DigiEquipSys.Pages
{
    public partial class ViewStock_pg
    {
        [CascadingParameter]
        public EventCallback notify { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                //myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                await notify.InvokeAsync();
            }
        }
        [Parameter]
        public string? vCompType { get; set; }

        WarningPage? Warning;
        string WarningHeaderMessage = "Warning!";
        string WarningContentMessage = "You must select a Warehouse";
        public bool SpinnerVisible { get; set; }=false;
        private SfGrid<Stock>? StockGrid;
        public Stock stock = new();
        protected List<GroupMaster> GroupMasterList = new();
        protected List<ItemMaster> ItemMasterList = new();
        protected List<Stock> StockList = new();
        protected List<SupplierMaster> SupplierMasterList = new();
        protected List<ClientMaster> ClientMasterList = new();
        public List<CategMaster>? ItemCatList = new();
        protected List<ItemUnit> ItemUnitList = new();
        protected List<VwItemBal> VendorDates = new();
        protected List<RcptDetail> RcptDetailList = new();
        private long stkid;
        private string? myLoc;
        private string? selectedLoc;
        private string? selectedItem;
        public bool IsVisRole { get; set; } = true;
        private string? myRole;
        public int TotalQty { get; set; }
        public decimal TotalAmt { get; set; }

        public class PurchDates
        {
            public decimal? StockBalQty { get; set; }
            public string? VendorInvNo { get; set; }
            public DateTime? VendorInvDate { get; set; }
            public decimal? VendorPurchQty { get; set; }
            public DateTime? ExpiryDate { get; set; }
        }
        public List<PurchDates> tblSource = new();

        public SfDialog? DialogVendorDates;

        public class GroupedRcptDetail
        {
            public string RdListNo { get; set; }
            public string RdLotNo { get; set; }
            public DateTime? RdExpiryDate { get; set; }
            public string RdClientCode { get; set; }
            public decimal RdQty { get; set; }
            public DateTime? RdVendInvDate { get; set; }
            public string RdVendInvNo { get; set; }
        }
        public List<GroupedRcptDetail> RcptTable = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                myRole = await sessionStorage.GetItemAsync<string>("adminRo");
                if (myRole == "01" || myRole=="02")
                {
                    IsVisRole = true;
                }
                else
                {
                    IsVisRole = false;
                }

                this.SpinnerVisible = true;
                await myStock.UpdateStockExpiryStatus();
                GroupMasterList = await myGroupMaster.GetGroupMasters();
                ItemMasterList = await myItemMaster.GetItemMasters();
                ItemCatList = await myCatService.GetCategMasters();
                ItemUnitList = await myUnit.GetItemUnits();
                StockList = await myStock.GetAllStocksNonZero();
                RcptDetailList = await myRcptService.GetRcptDetails();
                TotalQty = Convert.ToInt32(StockList.Sum(d => (d.ItemOpQty ?? 0) + (d.ItemTrInQty ?? 0) + (d.ItemPurQty ?? 0) - ((d.ItemDelQty ?? 0) + (d.ItemTrOutQty ?? 0))));
                TotalAmt = Convert.ToDecimal(StockList.Sum(d => ((d.ItemOpQty ?? 0) + (d.ItemTrInQty ?? 0) + (d.ItemPurQty ?? 0) - ((d.ItemDelQty ?? 0) + (d.ItemTrOutQty ?? 0))) * d.ItemCp));
                SupplierMasterList = await mySupplierService.GetSuppliers();
                ClientMasterList = await myClientService.GetClients();
                RcptTable = RcptDetailList.GroupBy(x => new { x.RdListNo, x.RdLotNo, x.RdExpiryDate, x.RdClientCode, x.RdVendInvDate, x.RdVendInvNo })
                .Select(g => new GroupedRcptDetail
                {
                    RdListNo = g.Key.RdListNo,
                    RdLotNo = g.Key.RdLotNo,
                    RdExpiryDate = g.Key.RdExpiryDate,
                    RdClientCode = g.Key.RdClientCode,
                    RdQty = (decimal)g.Sum(x => x.RdQty),
                    RdVendInvDate = g.Key.RdVendInvDate,
                    RdVendInvNo = g.Key.RdVendInvNo
                }).ToList();

                if (vCompType == "Expired")
                {
                    StockList = StockList.Where(x => x.ItemExpiryDate <= DateTime.Now).ToList();
                }
                else
                {
                    if (vCompType == "ExpireSoon")

                    {
                        StockList = StockList.Where(x => x.ItemExpiryDate > DateTime.Now && x.ItemExpiryDate <= DateTime.Now.AddMonths(2)).ToList();
                    }
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

        private async Task ShowItemPopup(long vItem)
        {
            try
            {
                //VendorDates = await myStock.GetVendDates(vItem);
                //if (VendorDates != null)
                //{
                //    await this.DialogVendorDates.ShowAsync();
                //}

                //VendorDates = await myStock.GetVendDates(vItem);
                //var fVendorDates = VendorDates.Where(x => x.ItemListNo != null && x.ItemLotNo != null && x.ItemExpiryDate != null && x.ItemClientCode != null).ToList();
                //var fRcptDetailList = RcptTable.Where(y => y.RdListNo != null && y.RdLotNo != null && y.RdExpiryDate != null && y.RdClientCode != null).ToList();
                //var data = (from x in fVendorDates
                //            join y in fRcptDetailList
                //                     on new { ListNo = (string)x.ItemListNo, LotNo = (string)x.ItemLotNo, ExpDate = (DateTime)x.ItemExpiryDate, ClientCode = (string)x.ItemClientCode }
                //                     equals new { ListNo = (string)y.RdListNo, LotNo = (string)y.RdLotNo, ExpDate = (DateTime)y.RdExpiryDate, ClientCode = (string)y.RdClientCode }
                //            select new PurchDates { VendorInvNo = y.RdVendInvNo, VendorInvDate = y.RdVendInvDate, VendorPurchQty = y.RdQty, StockBalQty = x.BalQty, ExpiryDate = x.ItemExpiryDate }).ToList();
                //tblSource = data;

                VendorDates = await myStock.GetVendDates(vItem);
                var fVendorDates = VendorDates.Where(x => x.ItemListNo != null && x.ItemLotNo != null && x.ItemExpiryDate != null && x.ItemClientCode != null).ToList();
                var fRcptDetailList = RcptTable.Where(y => y.RdListNo != null && y.RdLotNo != null && y.RdExpiryDate != null && y.RdClientCode != null).ToList();
                var data = (from x in fVendorDates
                            join y in fRcptDetailList
                                on new { ListNo = (string)x.ItemListNo, LotNo = (string)x.ItemLotNo, ExpDate = (DateTime)x.ItemExpiryDate, ClientCode = (string)x.ItemClientCode }
                                equals new { ListNo = (string?)y.RdListNo, LotNo = (string?)y.RdLotNo, ExpDate = (DateTime)y.RdExpiryDate, ClientCode = (string?)y.RdClientCode }
                                into gj
                            from y in gj.DefaultIfEmpty()
                            select new PurchDates
                            {
                                VendorInvNo = y?.RdVendInvNo,
                                VendorInvDate = y?.RdVendInvDate,
                                VendorPurchQty = y?.RdQty ?? 0,
                                StockBalQty = x.BalQty,
                                ExpiryDate = x.ItemExpiryDate
                            }).ToList();
                tblSource = data;
                await this.DialogVendorDates.ShowAsync();

                //await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        //private async Task<List<VwItemBal>> LoadPopupData(long vItemId)
        //{
        //    try
        //    {
        //        var data = await myStock.GetVendDates(vItemId);
        //        //var fVendorDates = VendorDates.Where(x => x.ItemListNo != null && x.ItemLotNo != null && x.ItemExpiryDate != null && x.ItemClientCode != null).ToList();
        //        //var fRcptDetailList = RcptDetailList.Where(y => y.RdListNo != null && y.RdLotNo != null && y.RdExpiryDate != null && y.RdClientCode != null).ToList();
        //        //var data = (from x in fVendorDates
        //        //            join y in fRcptDetailList
        //        //                     on new { ListNo = (string)x.ItemListNo, LotNo = (string)x.ItemLotNo, ExpDate = (DateTime)x.ItemExpiryDate, ClientCode = (string)x.ItemClientCode }
        //        //                     equals new { ListNo = (string)y.RdListNo, LotNo = (string)y.RdLotNo, ExpDate = (DateTime)y.RdExpiryDate, ClientCode = (string)y.RdClientCode }
        //        //            select new PurchDates { VendorInvNo = y.RdVendInvNo, VendorInvDate = y.RdVendInvDate, VendorPurchQty = y.RdQty, StockBalQty = x.BalQty }).ToList();
        //        return data ;
        //    }
        //    catch (Exception ex)
        //    {
        //        await JSRuntime.InvokeVoidAsync("alert", ex.Message);
        //        return null;
        //    }
        //}


        //public async Task ActionCompleteHandler(ActionEventArgs args)
        //{
        //if (args.Equals == Syncfusion.Blazor.Grids.Action.Refresh || args.RequestType == Action.Filtering)
        //    //{
        //    var filteredData = await StockGrid.GetCurrentViewRecordsAsync() as List<Stock>;
        //    if (filteredData != null)
        //    {
        //        TotalQty = Convert.ToInt32(filteredData.Sum(d => (d.ItemOpQty ?? 0) + (d.ItemTrInQty ?? 0) + (d.ItemPurQty ?? 0) - ((d.ItemDelQty ?? 0) + (d.ItemTrOutQty ?? 0))));

        //    }
        //    StateHasChanged(); // Refresh UI
        //    //}
        //}
        //private void OnGridDataBound()
        //{
        //    StockGrid?.AutoFitColumns();
        //}
        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Excel Export") //Id is combination of Grid's ID and itemname
            {
                try
                {
                    ExcelExportProperties ExportProperties = new ExcelExportProperties();
                    ExportProperties.IncludeTemplateColumn = true;
                    await this.StockGrid.ExportToExcelAsync(ExportProperties);
                    
                    this.SpinnerVisible = true;
                    if (StockGrid != null)
                    {
                        await StockGrid.ExportToExcelAsync();
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

        public async Task OnChangeItem(ChangedEventArgs args)
        {
            try
            {
                if (args.Value != null)
                {
                    selectedItem = args.Value;
                    this.SpinnerVisible = true;
                    StockList = await myStock.GetStocksByItemCode(args.Value);
                    TotalQty = Convert.ToInt32(StockList.Sum(d => (d.ItemOpQty ?? 0) + (d.ItemTrInQty ?? 0) + (d.ItemPurQty ?? 0) - ((d.ItemDelQty ?? 0) + (d.ItemTrOutQty ?? 0))));
                    TotalAmt = Convert.ToDecimal(StockList.Sum(d => ((d.ItemOpQty ?? 0) + (d.ItemTrInQty ?? 0) + (d.ItemPurQty ?? 0) - ((d.ItemDelQty ?? 0) + (d.ItemTrOutQty ?? 0))) * d.ItemCp));
                    this.SpinnerVisible = false;
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        public async Task NavigateToPrevious()
        {
            try
            {
                UriHelper.NavigateTo($"blank_pg");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        public void ExcelQueryCellInfoHandler(ExcelQueryCellInfoEventArgs<Stock> args)
        {
            if (args.Column.Field == "vBalQty")
            {
                args.Cell.Value = (args.Data.ItemOpQty ?? 0) + (args.Data.ItemTrInQty ?? 0) + (args.Data.ItemPurQty ?? 0) - ((args.Data.ItemDelQty ?? 0) +(args.Data.ItemTrOutQty ?? 0));
            }
        }
        public async Task OnQueryCellInfo(QueryCellInfoEventArgs<Stock> args)
        {
            try
            {
                if (args.Column.Field == "ItemExpiryDate")
                {
                    // Change the color based on the value in OrderAmount field
                    if (args.Data.ItemExpiryDate <= DateTime.Now)
                    {
                        args.Cell.AddStyle(new string[] { "background-color:white;color:Red" });
                    }
                    else
                    {
                        if (args.Data.ItemExpiryDate > DateTime.Now && args.Data.ItemExpiryDate <= DateTime.Now.AddMonths(2))
                        {
                            args.Cell.AddStyle(new string[] { "background-color:yellow;color:Green" });
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
