using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using DigiEquipSys.Services;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
namespace DigiEquipSys.Pages
{
    public partial class Po_pg
    {
        private string? myRole;
        private string? myLoc;
        
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

        public bool SpinnerVisible { get; set; }=false;
        WarningPage? Warning;
        string WarningHeaderMessage = "";
        string WarningContentMessage = "";

        ConfirmPage? DialogDelete;
        private string? ConfirmHeaderMessage = "Confirm Delete";
        private string? ConfirmContentMessage = "Please confirm that you want to Delete this P.O. ";

        private long PovouId;
        private long selectedPovouId { get; set; } = 0;
        [Inject]
        public IPoHeadService? PoHeadService { get; set; }
        public IEnumerable<PoHead>? PoVouList;
        [Inject]
        public ISupplierService? supplierService { get; set; }
        public IEnumerable<SupplierMaster>? suppList;

        private List<ItemModel> Toolbaritems = new();
        private SfGrid<PoHead>? PoHeadGrid;
        [Inject]
        public IPoDetailService? PoDetailService { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                myRole = await sessionStorage.GetItemAsync<string>("adminRo");
                this.SpinnerVisible = true;
                PoVouList = await PoHeadService.GetPoHeads();
                suppList = await supplierService.GetSuppliers();
                //await Task.Delay(1000);
                this.SpinnerVisible = false;
                Toolbaritems.Add(new ItemModel() { Text = "Add", TooltipText = "Add a new Purchase Order", PrefixIcon = "e-add" });
                Toolbaritems.Add(new ItemModel() { Text = "Edit", TooltipText = "Edit a selected Purchase Order", PrefixIcon = "e-edit" });
                Toolbaritems.Add(new ItemModel() { Text = "Delete", TooltipText = "Delete a selected Purchase Order", PrefixIcon = "e-edit" });
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        public void ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Add")
            {
                PovouId = 0;
                NavigationManager.NavigateTo($"poHead_pg/{PovouId}/");
            }

            if (args.Item.Text == "Edit")
            {
                if (selectedPovouId == 0)
                {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select a Purchase Order from the grid.";
                    Warning.OpenDialog();
                }
                else
                {
                    NavigationManager.NavigateTo($"poHead_pg/{selectedPovouId}/");
                }
            }

            if (args.Item.Text == "Delete")
            {
                args.Cancel = true;
                if (selectedPovouId == 0)
                {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select a Purchase Order Number from the grid.";
                    Warning.OpenDialog();
                }
                else
                {
                    if (selectedPovouId > 0)
                    {
                        var vMyApp = (from mytab in PoVouList where mytab.PohId == selectedPovouId select new { mytab.PohApproved }).FirstOrDefault();
                        if (vMyApp.PohApproved == true)
                        {
                            WarningHeaderMessage = "Warning!";
                            WarningContentMessage = "Selected Voucher is already in Approved Status and you can not delete";
                            Warning.OpenDialog();
                        }
                        else
                        {
                            DialogDelete.OpenDialog();
                        }
                    }
                    else
                    {
                        Warning.OpenDialog();
                    }
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
                    await PoDetailService.DeletePoDetail(selectedPovouId);
                    await PoHeadService.DeletePoHead(selectedPovouId);
                }
                this.SpinnerVisible = false;
                await PoHeadGrid.Refresh();
                NavigationManager.NavigateTo($"blank_pg");
                NavigationManager.NavigateTo($"po_pg");

            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        public void RowSelectHandler(RowSelectEventArgs<PoHead> args)
        {
            selectedPovouId = args.Data.PohId;
        }
        public void DispVouchers()
        {
            //NavigationManager.NavigateTo($"checkList_pg");
        }
        public void NavigateToPrevious()
        {
            NavigationManager.NavigateTo($"blank_pg");
        }
    }
}
