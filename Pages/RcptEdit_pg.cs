using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using DigiEquipSys.Services;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using System.Collections.Generic;

namespace DigiEquipSys.Pages
{
    public partial class RcptEdit_pg
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
        private long selectedRcptvouId { get; set; } = 0;
        private long RcptvouId;

        private List<ItemModel> Toolbaritems = new();

        [Inject]
        public IRcptHeadService? RcptHeadService { get; set; }
        public IEnumerable<RcptHead>? RcptVouList;

        protected override async Task OnInitializedAsync()
        {
            this.SpinnerVisible = true;
            try
            {
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                RcptVouList = await RcptHeadService.GetRcptHeads();
                await InvokeAsync(StateHasChanged);
                this.SpinnerVisible = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        public void NavigateToPrevious()
        {
            NavigationManager.NavigateTo($"blank_pg");
        }
        private async Task Navigate(long rcptId)
        {
            NavigationManager.NavigateTo($"rcptHeadEdit_pg/{rcptId}");

        }

    }
}
