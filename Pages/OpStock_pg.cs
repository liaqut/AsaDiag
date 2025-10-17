using DigiEquipSys.Models;
using Microsoft.AspNetCore.Components;
using DigiEquipSys.Shared;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Popups;
using Microsoft.JSInterop;

namespace DigiEquipSys.Pages
{
    public partial class OpStock_pg
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
        [Parameter]
        public string? vCompType { get; set; }

        WarningPage? Warning;
        string WarningHeaderMessage = "Warning!";
        string WarningContentMessage = "You must select a Warehouse";
        ConfirmPage? DialogDelete;
        string ConfirmHeaderMessage = "Confirm Delete";
        string ConfirmContentMessage = "Please confirm that you want to Delete this record ";
        private bool SpinnerVisible { get; set; }=false;
        private SfGrid<Stock>? StockGrid;
        public Stock stock = new Stock();
        protected List<GroupMaster> GroupMasterList = new();
        protected List<CategMaster> CategMasterList = new();
        protected List<ItemMaster> ItemMasterList = new();
        protected List<ItemUnit> ItemUnitList = new();
        protected List<SupplierMaster> SupplierMasterList = new();
        protected List<ClientMaster> ClientMasterList = new();
        protected List<Stock> StockList = new();
        //public List<ItemGroup>? ItemGroupList = new();
        //public List<ItemCat>? ItemCatList = new();
        private long stkid;
        private string? myRole;
        private string? myLoc;
        //private string? myDefWarehouse { get; set; }
        private SfDialog? DialogAddStock;
        public bool IsEdit { get; set; } = true;
        public bool IsVisRole { get; set; } = true;
        public ItemMaster itemmaster = new ItemMaster();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                myRole = await sessionStorage.GetItemAsync<string>("adminRo");
                if (myRole=="01")
                {
                    IsEdit = true;
                    IsVisRole = true;
                }
                else
                {
                    IsEdit = false;
                    IsVisRole = true;

                    if (myRole=="03")
                    {
                        IsVisRole = false;
                    }
                }

                this.SpinnerVisible = true;
                GroupMasterList = await myGroupMaster.GetGroupMasters();
                CategMasterList=await myCatMaster.GetCategMasters();
                ItemUnitList = await myUnit.GetItemUnits();
                ItemMasterList = await myItemMaster.GetItemMasters();
                //ItemGroupList = await myItemMaster.GetItemGroups();
                //ItemCatList=await myItemMaster.GetItemCats();
                StockList = await myStock.GetAllStocks();
                SupplierMasterList = await mySupplierService.GetSuppliers();
                ClientMasterList = await myClientService.GetClients();
                await InvokeAsync(StateHasChanged);
                this.SpinnerVisible = false;

                //var ItemMasterListForLoop = (from im in ItemMasterList where !StockList.Any(es => (es.ItemId == im.ItemId)) select im).ToList();
                //foreach (var qry in ItemMasterListForLoop)
                //{
                //    stock.StkId = 0;
                //    stock.ItemId = qry.ItemId;
                //    stock.ItemListNo=qry.ItemListNo;
                //    stock.ItemOpQty = 0;
                //    stock.ItemStkIdDesc = qry.ItemId;
                //    stock.ItemStkIdGrp = qry.ItemId;
                //    stock.ItemStkIdUnit = qry.ItemId;

                //    await myStock.CreateStock(stock);
                //    //Thread.Sleep(1000);
                //}
                //StockList = await myStock.GetAllStocks();
                //this.SpinnerVisible = false;
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
        public async Task ActionBeginHandler(ActionEventArgs<Stock> Args)
        {
            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Save))
            {
                if (Args.Data.StkId != 0)
                {
                    this.SpinnerVisible = true;
                    stkid = Args.Data.StkId;
                    var qry = (from bc in StockList where bc.StkId == stkid select bc).FirstOrDefault();
                    if (qry != null)
                    {
                        if (qry.StkId == stkid)
                        {
                            await myStock.UpdateStock(Args.Data); 
                        }
                        else
                        {
                            WarningContentMessage = "This Item Information is already exists! You can not overridden.";
                            Warning.OpenDialog();
                        }
                    }
                    this.SpinnerVisible = false;
                }
                else
                {
                    WarningContentMessage = "You must select a record";
                    Warning.OpenDialog();
                }
            }
        }
        public IEditorSettings myEditParams = new NumericEditCellParams
        {
            Params = new NumericTextBoxModel<object>() { ShowClearButton = true, ShowSpinButton = false }
        };
        public void NavigateToPrevious()
        {
            UriHelper.NavigateTo("index");

        }

        //public async Task onOpen(Syncfusion.Blazor.Popups.OpenEventArgs args)
        //{
        //    args.PreventFocus = true;
        //}

        private async Task AddStock()
        {
            stock = new Stock();
            await this.DialogAddStock.ShowAsync();
        }
        public void RowSelectHandler(RowSelectEventArgs<Stock> args)
        {
            stkid = args.Data.StkId;
        }
        private async Task DeleteStock()
        {
            if (stkid == 0)
            {
                WarningContentMessage = "You must select a Stock";
                Warning.OpenDialog();
            }
            else
            {
                ConfirmContentMessage = "Please confirm that you want to Delete this record " + stkid;
                DialogDelete.OpenDialog();
            }
        }
        protected async Task ConfirmDelete(bool DeleteConfirmed)
        {

            this.SpinnerVisible = true;
            if (DeleteConfirmed)
            {
                await myStock.DeleteStock(stkid); 
            }
            this.SpinnerVisible = false;
            StateHasChanged();
        }
        private async Task CloseDialog()
        {
            await this.DialogAddStock.HideAsync();
        }

        protected async Task StockSave()
        {
            try
            {
                this.SpinnerVisible = true;

                if (stock.StkId == 0)
                {
                    itemmaster = await myItemMaster.GetItemMaster(stock.ItemListNo, stock.ItemClientCode);
                    if (itemmaster != null)
                    {
                        stock.ItemPurAmt = 0;
                        stock.ItemPurQty = 0;
                        stock.ItemDelAmt = 0;
                        stock.ItemDelQty = 0;
                        stock.ItemUp = itemmaster.ItemCostPrice;
                        stock.ItemSp = itemmaster.ItemSellPrice;
                        stock.ItemStkIdDesc = itemmaster.ItemId;
                        stock.ItemStkIdGrp = itemmaster.ItemGrpCode;
                        stock.ItemStkIdCat = itemmaster.ItemCatCode;
                        stock.ItemStkIdUnit = itemmaster.ItemUnit;
                        if (stock.ItemExpiryDate <= DateTime.Now)
                        {
                            stock.ItemExpStat = "Yes";
                        }
                        else
                        {
                            stock.ItemExpStat = "No";
                        }
                        await myStock.CreateStock(stock);
                        this.StateHasChanged();
                        stock = new Stock();
                    }
                    else
                    {
                        WarningContentMessage = "Specified Item not found in the Item Master. Please add to the Master first...";
                        Warning.OpenDialog();
                    }
                }
                //else
                //{
                //    await myStock.UpdateStock(stock);
                //    await this.DialogAddStock.HideAsync();
                //    this.StateHasChanged();
                //}
                StockList = await myStock.GetAllStocks();
                this.SpinnerVisible = false;
                IsEdit = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
    }

}
