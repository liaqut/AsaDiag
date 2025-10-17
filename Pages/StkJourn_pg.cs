using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
namespace DigiEquipSys.Pages
{
    public partial class StkJourn_pg
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

        public bool SpinnerVisible { get; set; } = false;
        WarningPage? Warning;
        string WarningHeaderMessage = "";
        string WarningContentMessage = "";
        private long selectedTrvouId { get; set; } = 0;
        private long TrvouId;

        private List<ItemModel> Toolbaritems = new();

        [Inject]
        public ITrHeadService? TrHeadService { get; set; }
        public IEnumerable<TrHead>? TrVouList;

        protected override async Task OnInitializedAsync()
        {
            this.SpinnerVisible = true;
            try
            {
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                TrVouList = await TrHeadService.GetTrHeads();
                await InvokeAsync(StateHasChanged);
                this.SpinnerVisible = false;
                Toolbaritems.Add(new ItemModel() { Text = "Add", TooltipText = "Add a new GRN", PrefixIcon = "e-add" });
                Toolbaritems.Add(new ItemModel() { Text = "Edit", TooltipText = "Edit a selected GRN", PrefixIcon = "e-edit" });
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
                TrvouId = 0;
                NavigationManager.NavigateTo($"stkjournal_pg/{TrvouId}/");
            }

            if (args.Item.Text == "Edit")
            {
                if (selectedTrvouId == 0)
                {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select a Stock Journal Voucher from the grid.";
                    Warning.OpenDialog();
                }
                else
                {
                    NavigationManager.NavigateTo($"stkjournal_pg/{selectedTrvouId}/");
                }
            }
        }

        public void RowSelectHandler(RowSelectEventArgs<TrHead> args)
        {
            selectedTrvouId = args.Data.TrhId;
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
