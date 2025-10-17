using DigiEquipSys.Models;
using DigiEquipSys.Services;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Lists;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;
using System.Text.Json;
using static Syncfusion.XlsIO.Parser.Biff_Records.ExtendedFormatRecord;
//using Syncfusion.Blazor.Inputs;

namespace DigiEquipSys.Pages
{
    public partial class ItemMaster_pg
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
        private string? myLoc;
        private SfDropDownList<string, CategMaster> myCat;
        WarningPage? Warning;
        string WarningHeaderMessage = "Warning!";
        string WarningContentMessage = "You must select an Item";

        ConfirmPage? DialogDelete;
        string ConfirmHeaderMessage = "Confirm Delete";
        string ConfirmContentMessage = "Please confirm that you want to Delete this record ";

        ConfirmPurchPricePage? DialogPurchPrice;
        string PurchPriceHeaderMessage = "Confirm Purchase Price Changes";
        string PurchPriceContentMessage = "Please confirm that you want to Change this Purch Price ";

        ConfirmSellPricePage? DialogSellPrice;
        string SellPriceHeaderMessage = "Confirm Selling Price Changes";
        string SellPriceContentMessage = "Please confirm that you want to Change this Selling Price ";


        private bool SpinnerVisible { get; set; }=false;
        private DialogSettings DialogParams = new() { Width = "650px" };

        private SfGrid<ItemMaster>? ItemMasterGrid;
        public bool IsEdit { get; set; } = false;
        private long itemId;
        private string vgrpcode { get; set; } = "";
        protected List<ItemMaster> ItemMasterList = new();
        protected List<ItemMaster> ItemMasterListDistinct = new();
        protected List<ItemMaster> ItemMasterListTest = new();


        protected ItemMaster itemmaster = new();

        //protected List<GroupMaster> vGroupMasterList = new();
        protected List<CategMaster>? CategMasterList { get; set; }
        protected List<GroupMaster>? GroupMasterList { get; set; }
        //private List<string>? FilteredCategMasterList { get; set; } = new List<string>();

        protected List<ItemUnit> ItemUnitList = new();
        //public string[] ColumnItems = new string[] { "Code" };

        protected List<SupplierMaster> SupplierMasterList = new();
        protected List<ClientMaster> ClientMasterList = new();
        public bool Enabled = false;

        private SfDialog? DialogAddItemMaster;
        private List<ItemModel> Toolbaritems = new();
        private int sw { get; set; } = 0;
        private decimal? OldPurchPrice { get; set; } = 0.00M;
        private decimal? NewPurchPrice { get; set; } = 0.00M;

        private decimal? OldSellPrice { get; set; } = 0.00M;
        private decimal? NewSellPrice { get; set; } = 0.00M;
        private string? myRole;
        private string Custom { get; set; }
        private SfComboBox<string, ItemMaster> ComboItem;

        private string? vMyListNo { get; set; }
        private string? vMyProdNo { get; set; }
        private string? vMyItem { get; set; }
        private string? vMyGrp { get; set; }
        private string? vMyCat { get; set; }
        private string? vMyUnit { get; set; }
        private string? vMySupp { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                myRole = await sessionStorage.GetItemAsync<string>("adminRo");
                this.SpinnerVisible = true;
                ItemMasterList = await myItemMaster.GetItemMasters();  // await Http.GetFromJsonAsync<List<GenCountry>>("api/GenCountry");
                ItemMasterListDistinct = await myItemMaster.GetItemMastersDistinct();
                GroupMasterList = await myGroupMaster.GetGroupMasters();
                //vGroupMasterList = await myGroupMaster.GetGroupMasters();
                CategMasterList = await myCatMaster.GetCategMasters();
                //FilteredCategMasterList = await myCatMaster.GetDDCategMasters();
                ItemUnitList = await myItemUnit.GetItemUnits();
                SupplierMasterList=await mySupplierService.GetSuppliers();
                ClientMasterList = await myClientService.GetClients();

                //await Task.Delay(1000);
                this.SpinnerVisible = false;
                Toolbaritems.Add(new ItemModel() { Text = "Add", TooltipText = "Add a new Item Master", PrefixIcon = "e-add" });
                Toolbaritems.Add(new ItemModel() { Text = "Edit", TooltipText = "Edit a selected Item Master", PrefixIcon = "e-edit" });
                Toolbaritems.Add(new ItemModel() { Text = "Delete", TooltipText = "Delete a Record", PrefixIcon = "e-delete" });
                Toolbaritems.Add(new ItemModel() { Text = "Export", TooltipText = "Export to Excel", PrefixIcon = "e-export" });
                Toolbaritems.Add(new ItemModel() { Text = "ZohoUpdate", TooltipText = "Append Vendors to Zoho ", PrefixIcon = "e-export" });

                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
				await JSRuntime.InvokeVoidAsync("alert", ex.Message);
				return;
			}
		}

        private void ConnectToZoho()
        {
            var returnUrl = UriHelper.ToBaseRelativePath(UriHelper.Uri);
            var url = ZohoTokenService.GetAuthUrl(returnUrl);
            UriHelper.NavigateTo(url, true);
        }
        private async Task ChangePurchPrice(Microsoft.AspNetCore.Components.ChangeEventArgs args)
        {
            if (itemmaster != null)
            {
                if (OldPurchPrice != 0.00M)
                {
                    PurchPriceHeaderMessage = "Changing Purchase Price";
                    PurchPriceContentMessage = "Please confirm that you want to Change From " + OldPurchPrice.ToString() + " To " + args.Value.ToString();
                    DialogPurchPrice?.OpenDialog();

                    if (decimal.TryParse(args.Value.ToString(), out decimal newPurchPrice))
                    {
                        NewPurchPrice = newPurchPrice;
                        await InvokeAsync(StateHasChanged);
                    }
                    else
                    {
                        await JSRuntime.InvokeVoidAsync("alert", "Invalid decimal part");
                        return;
                    }
                }
            }
        }

        protected async Task ConfirmPurchPrice(bool PurchPriceChangeConfirmed)
        {
            try
            {
                this.SpinnerVisible = true;
                if (PurchPriceChangeConfirmed)
                {
                    itemmaster.ItemCostPricePrev = OldPurchPrice;
                    itemmaster.ItemCostPrice = NewPurchPrice;
                    ItemMasterList = await myItemMaster.GetItemMasters();
                }
                else
                {
                    itemmaster.ItemCostPrice = OldPurchPrice;
                }
                this.SpinnerVisible = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }


        private async Task ChangeSellPrice(Microsoft.AspNetCore.Components.ChangeEventArgs args)
        {
            if (itemmaster != null)
            {
                if (OldSellPrice != 0.00M)
                {
                    SellPriceHeaderMessage = "Changing Selling Price";
                    SellPriceContentMessage = "Please confirm that you want to Change From " + OldSellPrice.ToString() + " To " + args.Value.ToString();
                    DialogSellPrice?.OpenDialog();

                    if (decimal.TryParse(args.Value.ToString(), out decimal newSellPrice))
                    {
                        NewSellPrice = newSellPrice;
                        await InvokeAsync(StateHasChanged);
                    }
                    else
                    {
                        await JSRuntime.InvokeVoidAsync("alert", "Invalid decimal part");
                        return;
                    }
                }
            }
        }

        protected async Task ConfirmSellPrice(bool SellPriceChangeConfirmed)
        {
            try
            {
                this.SpinnerVisible = true;
                if (SellPriceChangeConfirmed)
                {
                    itemmaster.ItemSellPricePrev = OldSellPrice;
                    itemmaster.ItemSellPrice = NewSellPrice;
                    ItemMasterList = await myItemMaster.GetItemMasters();
                }
                else
                {
                    itemmaster.ItemSellPrice = OldSellPrice;
                }
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
            if (args.Item.Text == "Export") 
            {
                try
                {
                    this.SpinnerVisible = true;
                    if (ItemMasterGrid != null)
                    {
                        await ItemMasterGrid.ExportToExcelAsync();
                    }
                    this.SpinnerVisible = false;
                }
                catch (Exception ex)
                {
					await JSRuntime.InvokeVoidAsync("alert", ex.Message);
					return;
				}
			}

            if (args.Item.Text == "Delete")
            {
                try
                {
                    args.Cancel = true;
                    if (itemId != 0)
                    {
                        DialogDelete?.OpenDialog();
                    }
                    else
                    {
                        Warning?.OpenDialog();
                    }
                }
                catch (Exception ex)
                {
					await JSRuntime.InvokeVoidAsync("alert", ex.Message);
					return;
				}
			}

            if (args.Item.Text == "Add")
            {
                IsEdit = true;
                Enabled = true;
                itemmaster = new ItemMaster();
                await this.DialogAddItemMaster.ShowAsync();
            }

            if (args.Item.Text == "Edit")
            {
                IsEdit = false;
                Enabled = false;
                if (itemId == 0)
                {
                    WarningContentMessage = "You must select an Item";
                    Warning.OpenDialog();
                }
                else
                {
                    IsEdit = false;
                    this.SpinnerVisible = true;
                    itemmaster = await myItemMaster.GetItemMaster(itemId);
                    await this.DialogAddItemMaster.ShowAsync();
                    this.SpinnerVisible = false;
                }
            }
            if (args.Item.Text == "ZohoUpdate")
            {
                try
                {
                    SpinnerVisible = true;
                    var token = await ZohoTokenService.GetAccessTokenAsync();
                    var orgId = await ZohoService.GetOrganizationIdAsync(token);

                    ItemMasterListTest = await myItemMaster.GetItemMastersTest();
                    foreach (var item in ItemMasterListTest)
                    {
                        if (item.ItemZohoItemId == null || item.ItemZohoItemId == "")
                        {

                            var ItemData = new
                            {
                                name = item.ItemListNo+"-"+item.ItemClientCode,
                                status = "active",
                                description = item.ItemDesc,
                                rate = 0.00,
                                unit = item.ItemUnit,
                                hsn_or_sac = item.ItemHsnCode,
                                tax_type="GST",
                                tax_percentage=item.ItemGst,
                                item_type="inventory"

                            };

                            var result = await ZohoService.AppendItemToZoho(token, orgId, ItemData);

                            if (result != null)
                            {
                                using var doc = JsonDocument.Parse(result);
                                var itemId = doc.RootElement
                                                   .GetProperty("item")
                                                   .GetProperty("item_id")
                                                   .GetString();

                                item.ItemZohoItemId = itemId;
                                await myItemMaster.UpdateItemMaster(item);
                            }
                        }
                    }
                    SpinnerVisible = false;
                }
                catch (Exception ex)
                {
                    await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                    return;
                }
            }

        }

        private async Task CloseDialog()
        {
            await this.DialogAddItemMaster.HideAsync();
        }

        protected async Task ItemMasterSave()
        {
            try
            {
                sw = 0;



                //Console.WriteLine("ItemClientCode: " + itemmaster.ItemClientCode);
                //Console.WriteLine("ItemDesc: " + itemmaster.ItemDesc);
                //Console.WriteLine("ItemUnit: " + itemmaster.ItemUnit);
                //Console.WriteLine("ItemSuppCode: " + itemmaster.ItemSuppCode);
                //Console.WriteLine("ItemSellPrice: " + itemmaster.ItemSellPrice);
                //Console.WriteLine("ItemCostPrice: " + itemmaster.ItemCostPrice);

                if (itemmaster.ItemClientCode == null || itemmaster.ItemClientCode == "")
                {
                    WarningContentMessage = "You must select a Client";
                    Warning.OpenDialog();
                    sw = 1;

                }

                if (itemmaster.ItemDesc == null || itemmaster.ItemDesc == "")
                {
                    WarningContentMessage = "Fill Item Description";
                    Warning.OpenDialog();
                    sw = 1;
                }

                if (itemmaster.ItemUnit == null || itemmaster.ItemUnit == "")
                {
                    WarningContentMessage = "Fill Item Unit";
                    Warning.OpenDialog();
                    sw = 1;
                }


                if (itemmaster.ItemSuppCode == null || itemmaster.ItemSuppCode =="")
                {
                    WarningContentMessage = "You must select a Supplier";
                    Warning.OpenDialog();
                    sw = 1;
                }

                if (itemmaster.ItemSellPrice == 0 || itemmaster.ItemSellPrice == null)
                {
                    WarningContentMessage = "Enter a selling price...";
                    Warning.OpenDialog();
                    sw = 1;
                }
                if (itemmaster.ItemCostPrice == 0 || itemmaster.ItemCostPrice == null)
                {
                    WarningContentMessage = "Enter a Purchase price...";
                    Warning.OpenDialog();
                    sw = 1;
                }

                if (sw == 0)
                {
                    this.SpinnerVisible = true;
                    if (itemmaster.ItemId == 0)
                    {
                        await myItemMaster.CreateItemMaster(itemmaster);
                        this.StateHasChanged();
                        itemmaster = new ItemMaster();

                        itemmaster.ItemListNo = vMyListNo;
                        itemmaster.ItemDesc = vMyItem;
                        itemmaster.ItemProdCode = vMyProdNo;
                        itemmaster.ItemUnit = vMyUnit;
                        itemmaster.ItemGrpCode = vMyGrp;
                        itemmaster.ItemCatCode = vMyCat;
                        itemmaster.ItemSuppCode = vMySupp;
                    }
                    else
                    {
                        await myItemMaster.UpdateItemMaster(itemmaster);
                        await this.DialogAddItemMaster.HideAsync();
                        this.StateHasChanged();
                    }
                    ItemMasterList = await myItemMaster.GetItemMasters();
                    this.SpinnerVisible = false;
                    IsEdit = false;
                }
            }
            catch (Exception ex)
            {
				await JSRuntime.InvokeVoidAsync("alert", ex.Message);
				return;
			}
		}

        private async Task OnCatOpen()
        {
            CategMasterList = await myCatMaster.GetCategMasters(vgrpcode);
        }
        private async Task OnGroupChanged(ChangeEventArgs<string, GroupMaster> args)
        {
            if (args.Value != null)
            {
                //var qry = (from z in GroupMasterList where z.GrpNo== args.Value select z).FirstOrDefault();
                //if (qry !=  null)
                //{
                CategMasterList = await myCatMaster.GetCategMasters(args.Value);
                this.itemmaster.ItemCatCode = null;
                //await Task.Delay(3000);
                StateHasChanged();
                //}

            }
        }
        public void RowSelectHandler(RowSelectEventArgs<ItemMaster> args)
        {
            OldPurchPrice = args.Data.ItemCostPrice;
            OldSellPrice = args.Data.ItemSellPrice;
            itemId = args.Data.ItemId;
            vgrpcode = args.Data.ItemGrpCode;
        }

        protected async Task ConfirmDelete(bool DeleteConfirmed)
        {
            this.SpinnerVisible = true;
            if (DeleteConfirmed)
            {
                await myItemMaster.DeleteItemMaster(itemId); //await Http.DeleteAsync("api/GenCountry/" + countryId);
            }
            ItemMasterList = await myItemMaster.GetItemMasters(); //await Http.GetFromJsonAsync<List<GenCountry>>("api/GenCountry");
            this.SpinnerVisible = false;
            IsEdit = false;
        }

        public void NavigateToPrevious()
        {
            UriHelper.NavigateTo($"blank_pg");
        }

        public async Task OnItemChanged(ChangeEventArgs<string, ItemMaster> args)
        {
            try
            {
                if (args.Value != null)
                {
                    itemmaster.ItemListNo = args.ItemData.ItemListNo;
                    itemmaster.ItemDesc = args.ItemData.ItemDesc;
                    itemmaster.ItemProdCode = args.ItemData.ItemProdCode;
                    itemmaster.ItemUnit = args.ItemData.ItemUnit;
                    itemmaster.ItemGrpCode = args.ItemData.ItemGrpCode;
                    itemmaster.ItemCatCode = args.ItemData.ItemCatCode;
                    itemmaster.ItemSuppCode = args.ItemData.ItemSuppCode;

                    vMyListNo = args.ItemData.ItemListNo;
                    vMyProdNo = args.ItemData.ItemProdCode;
                    vMyItem  = args.ItemData.ItemDesc;
                    vMyGrp  = args.ItemData.ItemGrpCode;
                    vMyCat  = args.ItemData.ItemCatCode;
                    vMyUnit  = args.ItemData.ItemUnit;
                    vMySupp = args.ItemData.ItemSuppCode;

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
            //Custom = args.Text;
            //args.PreventDefaultAction = true;
            //var query = new Query().Where(new WhereFilter() { Field = "ItemDesc", Operator = "contains", value = args.Text, IgnoreCase = true });
            //query = !string.IsNullOrEmpty(args.Text) ? query : new Query();
            //await ComboItem.FilterAsync(ItemMasterListDistinct, query);

            args.PreventDefaultAction = true;
            Query query = new Query();

            if (!string.IsNullOrEmpty(args.Text))
            {
                query = query.Where(new WhereFilter
                {
                    Field = "ItemDesc",
                    Operator = "contains",
                    value = args.Text,
                    IgnoreCase = true
                });
            }

            await ComboItem.FilterAsync(ItemMasterListDistinct, query);

        }
    }
}
