using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using System.Collections.Generic;
namespace DigiEquipSys.Pages
{
    public partial class Ssale_pg
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
        private long DelnoteId;
        private string selectedDelnote { get; set; } = "";
        [Inject]
        public ISDelHeadService? SDelHeadService { get; set; }
        public IEnumerable<SdelHead>? Delnotelist;
        private long selectedDelnoteId { get; set; } = 0;

        protected AdminInfo admininfo = new();
        private List<ItemModel> Toolbaritems = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                this.SpinnerVisible = true;
                //Delnotelist = await DelHeadService.GetDelHeadSale();
                Delnotelist = await SDelHeadService.GetSdelHeads();
                await InvokeAsync(StateHasChanged);
                this.SpinnerVisible = false;
                Toolbaritems.Add(new ItemModel() { Text = "Add", TooltipText = "Add a new Delivery Note", PrefixIcon = "e-add" });
                Toolbaritems.Add(new ItemModel() { Text = "Edit", TooltipText = "Edit a selected Delivery Note", PrefixIcon = "e-edit" });

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
                DelnoteId = 0;
                NavigationManager.NavigateTo($"ssaleHead_pg/{DelnoteId}");
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
                    NavigationManager.NavigateTo($"ssaleHead_pg/{selectedDelnoteId}");
                }
            }
        }
        public void NavigateToPrevious()
        {
            NavigationManager.NavigateTo($"blank_pg");
        }

        public void RowSelectHandler(RowSelectEventArgs<SdelHead> args)
        {
            selectedDelnoteId = args.Data.SdelId;
        }

        private async Task Navigate(long detCode)
        {
            NavigationManager.NavigateTo($"ssaleHead_pg/{detCode}");
            //NavigationManager.NavigateTo($"ssaleHead_pg/{Uri.EscapeDataString(detCode)}");
        }

    }
}
