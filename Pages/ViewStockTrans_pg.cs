using DigiEquipSys.Models;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Navigations;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace DigiEquipSys.Pages
{
	public partial class ViewStockTrans_pg
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
        public bool SpinnerVisible { get; set; } = false;
        private SfGrid<StockTran>? StockGrid;
        public StockTran stock = new();
        protected List<GroupMaster> GroupMasterList = new();
        protected List<ItemMaster> ItemMasterList = new();
        protected List<StockTran> StockList = new();
        protected List<SupplierMaster> SupplierMasterList = new();
        protected List<ClientMaster> ClientMasterList = new();
        public List<CategMaster>? ItemCatList = new();
        protected List<ItemUnit> ItemUnitList = new();
        private long stkid;
        private string? myLoc;
        private string? selectedLoc;
        private string? selectedItem;
        public bool IsVisRole { get; set; } = true;
        private string? myRole;
        public int TotalQty { get; set; }
        public decimal TotalAmt { get; set; }

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
                //DateTime StDate = DateTime.Today.AddDays(-30);
                //DateTime EnDate = DateTime.Today;

                GroupMasterList = await myGroupMaster.GetGroupMasters();
                ItemMasterList = await myItemMaster.GetItemMasters();
                ItemCatList = await myCatService.GetCategMasters();
                ItemUnitList = await myUnit.GetItemUnits();
                //StockList = await myStock.GetStockTransByDate(StDate,EnDate);
                //TotalQty = Convert.ToInt32(StockList.Sum(d => (d.ItemTrOpQty ?? 0) + (d.ItemTrInQty ?? 0) + (d.ItemPurQty ?? 0) - ((d.ItemDelQty ?? 0) + (d.ItemTrOutQty ?? 0))));
                SupplierMasterList = await mySupplierService.GetSuppliers();
                ClientMasterList = await myClientService.GetClients();
                await InvokeAsync(StateHasChanged);
                this.SpinnerVisible = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

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


        public async Task ValueChangeHandler(RangePickerEventArgs<DateTime?> args)
        {
            DateTime StDate = args.StartDate.Value;
            DateTime EnDate = args.EndDate.Value;
            StockList = await myStock.GetStockTransByDate(StDate.AddDays(0), EnDate.AddDays(1));
            await InvokeAsync(StateHasChanged);
            TotalQty = Convert.ToInt32(StockList.Sum(d => (d.ItemTrOpQty ?? 0) + (d.ItemTrInQty ?? 0) + (d.ItemPurQty ?? 0) - ((d.ItemDelQty ?? 0) + (d.ItemTrOutQty ?? 0))));
            TotalAmt = Convert.ToDecimal(StockList.Sum(d => ((d.ItemTrOpQty ?? 0) + (d.ItemTrInQty ?? 0) + (d.ItemPurQty ?? 0) - ((d.ItemDelQty ?? 0) + (d.ItemTrOutQty ?? 0))) * d.ItemCp));
            StockGrid.Refresh();
        }

        //public async Task OnChangeItem(ChangedEventArgs args)
        //{
            //try
            //{
            //    if (args.Value != null)
            //    {
            //        selectedItem = args.Value;
            //        this.SpinnerVisible = true;
            //        StockList = await myStock.GetStocksByItemCode(args.Value);
            //        TotalQty = Convert.ToInt32(StockList.Sum(d => (d.ItemOpQty ?? 0) + (d.ItemTrInQty ?? 0) + (d.ItemPurQty ?? 0) - ((d.ItemDelQty ?? 0) + (d.ItemTrOutQty ?? 0))));
            //        this.SpinnerVisible = false;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    await JSRuntime.InvokeVoidAsync("alert", ex.Message);
            //    return;
            //}
        //}
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

        public void ExcelQueryCellInfoHandler(ExcelQueryCellInfoEventArgs<StockTran> args)
        {
            if (args.Column.Field == "vBalQty")
            {
                args.Cell.Value = (args.Data.ItemTrOpQty ?? 0) + (args.Data.ItemTrInQty ?? 0) + (args.Data.ItemPurQty ?? 0) - ((args.Data.ItemDelQty ?? 0) + (args.Data.ItemTrOutQty ?? 0));
            }
        }

    }
}
