using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using DigiEquipSys.Services;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
namespace DigiEquipSys.Pages
{
    public partial class Rcpt_pg
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
        private string? ConfirmContentMessage = "Please confirm that you want to Delete this GRN ";

        private long selectedRcptvouId { get; set; } = 0;
        private long RcptvouId;

        private List<ItemModel> Toolbaritems = new();

        [Inject]
        public IRcptHeadService? RcptHeadService { get; set; }
        public IEnumerable<RcptHead>? RcptVouList;
        [Inject]
        public IRcptDetailService? RcptDetailService { get; set; }

        private SfGrid<RcptHead>? RcptHeadGrid;

        protected override async Task OnInitializedAsync()
        {
            this.SpinnerVisible = true;
            try
            {
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                RcptVouList = await RcptHeadService.GetRcptHeads();
                //suppList = await SupplierService.GetSuppliers();
                this.SpinnerVisible = false;
                Toolbaritems.Add(new ItemModel() { Text = "AddFast", TooltipText = "Add a new GRN (Fast)", PrefixIcon = "e-add" });
                Toolbaritems.Add(new ItemModel() { Text = "EditFast", TooltipText = "Edit a GRN (Fast)", PrefixIcon = "e-add" });
                Toolbaritems.Add(new ItemModel() { Text = "Add", TooltipText = "Add a new GRN", PrefixIcon = "e-add" });
                Toolbaritems.Add(new ItemModel() { Text = "Edit", TooltipText = "Edit a selected GRN", PrefixIcon = "e-edit" });
                Toolbaritems.Add(new ItemModel() { Text = "Delete", TooltipText = "Delete a selected GRN", PrefixIcon = "e-edit" });
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

            if (args.Item.Text == "AddFast")
            {
                RcptvouId = 0;
                NavigationManager.NavigateTo($"rcptHeadFast_pg/{RcptvouId}/" + vCompType);
            }
            if (args.Item.Text == "EditFast")
            {
                if (selectedRcptvouId == 0)
                {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select a Purchase (Goods Receipt Note) from the grid.";
                    Warning.OpenDialog();
                }
                else
                {
                    NavigationManager.NavigateTo($"rcptHeadFast_pg/{selectedRcptvouId}/" + vCompType);
                }
            }
            if (args.Item.Text == "Add")
            {
                RcptvouId = 0;
                NavigationManager.NavigateTo($"rcptHead_pg/{RcptvouId}/" + vCompType);
            }

            if (args.Item.Text == "Edit")
            {
                if (selectedRcptvouId == 0)
                {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select a Purchase (Goods Receipt Note) from the grid.";
                    Warning.OpenDialog();
                }
                else
                {
                    NavigationManager.NavigateTo($"rcptHead_pg/{selectedRcptvouId}/" + vCompType);
                }
            }
            if (args.Item.Text == "Delete")
            {
                args.Cancel = true;
                if (selectedRcptvouId == 0)
                {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select a Purchase (Goods Receipt Note) from the grid.";
                    Warning.OpenDialog();
                }
                else
                {
                    if (selectedRcptvouId > 0)
                    {
                        var vMyApp = (from mytab in RcptVouList where mytab.RhId == selectedRcptvouId select new { mytab.RhApproved}).FirstOrDefault();
                        if (vMyApp.RhApproved == true)
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
                    await RcptDetailService.DeleteRcptDetailbyRcptHead(selectedRcptvouId);
                    await RcptHeadService.DeleteRcptHead(selectedRcptvouId);
                }
                this.SpinnerVisible = false;
                await RcptHeadGrid.Refresh();
                NavigationManager.NavigateTo($"blank_pg");
                NavigationManager.NavigateTo($"rcpt_pg");

            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        public void RowSelectHandler(RowSelectEventArgs<RcptHead> args)
        {
            selectedRcptvouId = args.Data.RhId;
        }
        public void DispVouchers()
        {
        }
        public void NavigateToPrevious()
        {
            NavigationManager.NavigateTo($"blank_pg");
        }
    }
}
