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
    public partial class Del_pg
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
        public string? myRole;
        public string? myLoc;
        public string? vUser;
        public bool SpinnerVisible { get; set; } = false;

        WarningPage? Warning;
        string WarningHeaderMessage = "";
        string WarningContentMessage = "";
        ConfirmPage? DialogDelete;
        private string? ConfirmHeaderMessage = "Confirm Delete";
        private string? ConfirmContentMessage = "Please confirm that you want to Delete this Delivery Note ";

        private SfGrid<DelHeadResult>? DelHeadGrid;

        private long DelnoteId;
        private long selectedDelnoteId { get; set; } = 0;
        [Inject]
        public IDelHeadService? DelHeadService { get; set; }
        public IEnumerable<DelHeadResult>? Delnotelist;

        [Inject]
        public IDelDetlService? DelDetlService { get; set; }
        public IEnumerable<DelDetl>? DelDetllist;

        [Inject]
        public IClientService? ClientService { get; set; }
        public IEnumerable<ClientMaster>? clientlist;

        [Inject]
        public IGenCityService? myCityService { get; set; }
        public IEnumerable<GenCity>? citylist;

        private List<ItemModel> Toolbaritems = new();

        protected AdminInfo admininfo = new();

        public class DelHeadResult
        {
            public long? DelId { get; set; }
            public string? DelDispNo { get; set; }
            public DateTime? DelDate { get; set; }
            public int? DelClientId { get; set; }
            public int? DelCityId { get; set; }
            public string? PoNumber { get; set; }
            public bool? DelApproved { get; set; }
            public decimal? DiffAmt { get; set; }
            public decimal? Margin { get; set; }
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                myRole = await sessionStorage.GetItemAsync<string>("adminRo");

                this.SpinnerVisible = true;
                Delnotelist = await DelHeadService.GetDelHeadResults();
                clientlist = await ClientService.GetClients();
                citylist = await myCityService.GetGenCityDetails();
                this.SpinnerVisible = false;
                Toolbaritems.Add(new ItemModel() { Text = "AddFast", TooltipText = "Add a new Delivery Note (Fast)", PrefixIcon = "e-add" });
                Toolbaritems.Add(new ItemModel() { Text = "EditFast", TooltipText = "Edit a Delivery Note (Fast)", PrefixIcon = "e-add" });
                Toolbaritems.Add(new ItemModel() { Text = "Add", TooltipText = "Add a new Delivery Note", PrefixIcon = "e-add" });
                Toolbaritems.Add(new ItemModel() { Text = "Edit", TooltipText = "Edit a selected Delivery Note", PrefixIcon = "e-edit" });
                Toolbaritems.Add(new ItemModel() { Text = "Delete", TooltipText = "Delete a selected Delivery Note", PrefixIcon = "e-edit" });
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
                DelnoteId = 0;
                NavigationManager.NavigateTo($"delHeadFast_pg/{DelnoteId}/" + vCompType);
            }
            if (args.Item.Text == "EditFast")
            {
                if (selectedDelnoteId == 0)
                {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select a Delivery Note from the grid.";
                    Warning.OpenDialog();
                }
                else
                {
                    NavigationManager.NavigateTo($"delHeadFast_pg/{selectedDelnoteId}/" + vCompType);
                }
            }

            if (args.Item.Text == "Add")
            {
                DelnoteId = 0;
                NavigationManager.NavigateTo($"DelHead_pg/{DelnoteId}/{vCompType}");
            }

            if (args.Item.Text == "Edit")
            {
                if (selectedDelnoteId == 0)
                {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select a Delivery Order from the grid.";
                    Warning.OpenDialog();
                }
                else
                {
                    NavigationManager.NavigateTo($"DelHead_pg/{selectedDelnoteId}/{vCompType}");
                }
            }
            if (args.Item.Text == "Delete")
            {
                args.Cancel = true;
                if (selectedDelnoteId == 0)
                {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select a Delivery Note from the grid.";
                    Warning.OpenDialog();
                }
                else
                {
                    if (selectedDelnoteId > 0)
                    {
                        var vMyApp = (from mytab in Delnotelist where mytab.DelId == selectedDelnoteId select new { mytab.DelApproved }).FirstOrDefault();
                        if (vMyApp.DelApproved == true)
                        {
                            WarningHeaderMessage = "Warning!";
                            WarningContentMessage = "Selected Voucher is already Approved Status and you can not delete";
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
                    await DelDetlService.DeleteDelDetlbyDelHead(selectedDelnoteId);
                    await DelHeadService.DeleteDelHead(selectedDelnoteId);
                }
                this.SpinnerVisible = false;
                await DelHeadGrid.Refresh();
                NavigationManager.NavigateTo($"blank_pg");
                NavigationManager.NavigateTo($"del_pg");

            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        public void RowSelectHandler(RowSelectEventArgs<DelHeadResult> args)
        {
            selectedDelnoteId = (long)args.Data.DelId;
        }

        public void NavigateToPrevious()
        {
            NavigationManager.NavigateTo($"blank_pg");
        }

    }
}
