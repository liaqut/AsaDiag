using DigiEquipSys.Models;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Popups;

namespace DigiEquipSys.Pages
{
    public partial class PoRcvd_pg
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

        WarningPage? Warning;
        string WarningHeaderMessage = "Warning!";
        string WarningContentMessage = "You must select a Warehouse";
        public bool SpinnerVisible { get; set; } = false;
        private SfGrid<VwPurchaseOrder>? PoGrid;
        public List<VwPurchaseOrder>? PoList = new();
        protected List<RcptDetail> RcptDetailList = new();

        private string? myLoc;
        public bool IsVisRole { get; set; } = true;
        private string? myRole;
        public int TotalQty { get; set; }
        public decimal TotalAmt { get; set; }
        public decimal TotalRcvd { get; set; }
        public decimal TotalRcvdAmt { get; set; }

        //public class ReceiptDates
        //{
        //    public string? VendorInvNo { get; set; }
        //    public DateTime? VendorInvDate { get; set; }
        //    public decimal? VendorPurchQty { get; set; }
        //}
        //public List<ReceiptDates> tblSource = new();
        public SfDialog? popupDialog;
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
                PoList = await myPoDetailService.GetvwPoByDate(StDate.AddDays(0), EnDate.AddDays(1));
                await InvokeAsync(StateHasChanged);
                TotalQty = Convert.ToInt32(PoList.Sum(d => (d.PoQty ?? 0)));
                TotalAmt = Math.Round(PoList.Sum(d => (d.PoTotal ?? 0)), 2);
                TotalRcvd = Math.Round(PoList.Sum(d => (d.PoRcvdQty ?? 0)), 2);
                TotalRcvdAmt = Math.Round(PoList.Sum(d => (d.PoRcvdTotal ?? 0)), 2);
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
                    if (PoGrid != null)
                    {
                        await PoGrid.ExportToExcelAsync();
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
            PoList = await myPoDetailService.GetvwPoByDate(StDate.AddDays(0), EnDate.AddDays(1));
            await InvokeAsync(StateHasChanged);
            TotalQty = Convert.ToInt32(PoList.Sum(d => (d.PoQty ?? 0)));
            TotalAmt = Math.Round(PoList.Sum(d => (d.PoTotal ?? 0)), 2);
            TotalRcvd = Math.Round(PoList.Sum(d => (d.PoRcvdQty ?? 0)), 2);
            PoGrid.Refresh();
        }
        public void NavigateToPrevious()
        {
            UriHelper.NavigateTo($"blank_pg");
        }

        private async Task ShowPopup(VwPurchaseOrder po)
        {
            try
            {
                RcptDetailList = await myRcptService.GetRcptDetails(po.PodListNo, po.ClientCode, po.PohDispNo);
                await this.popupDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
    }
}
