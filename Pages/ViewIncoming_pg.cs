using DigiEquipSys.Models;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;

namespace DigiEquipSys.Pages
{
    public partial class ViewIncoming_pg
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

        WarningPage? Warning;
        string WarningHeaderMessage = "Warning!";
        string WarningContentMessage = "You must select a Warehouse";
        public bool SpinnerVisible { get; set; } = false;
        private SfGrid<VwReceipt>? IncomingGrid;
        public List<VwReceipt>? IncomingList = new();
        private long stkid;
        private string? myLoc;
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
				DateTime StDate = Convert.ToDateTime("01/" + DateTime.Now.Month.ToString("00") + "/" + DateTime.Now.Year);
				DateTime EnDate = DateTime.Now;
				IncomingList = await myvwReceiptService.GetvwReceiptsDate(StDate.AddDays(0), EnDate.AddDays(1));
				//IncomingList = await myvwReceiptService.GetvwReceipts();
                await InvokeAsync(StateHasChanged);
                TotalQty = Convert.ToInt32(IncomingList.Sum(d => (d.RdQty ?? 0)));
                TotalAmt = Math.Round(IncomingList.Sum(d => (d.RdTotal ?? 0)), 2);
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
                    this.SpinnerVisible = true;
                    if (IncomingGrid != null)
                    {
                        await IncomingGrid.ExportToExcelAsync();
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
            IncomingList = await myvwReceiptService.GetvwReceiptsDate(StDate.AddDays(0),EnDate.AddDays(1));
            await InvokeAsync(StateHasChanged);
            TotalQty = Convert.ToInt32(IncomingList.Sum(d => (d.RdQty ?? 0)));
            TotalAmt = Math.Round(IncomingList.Sum(d => (d.RdTotal ?? 0)), 2);
            IncomingGrid.Refresh();
        }
        public void NavigateToPrevious()
        {
            UriHelper.NavigateTo($"blank_pg");
        }
    }
}
