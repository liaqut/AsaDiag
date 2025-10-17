using DigiEquipSys.Models;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using System.Globalization;

namespace DigiEquipSys.Pages
{
    public partial class ViewOutgoing_pg
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

        private SfGrid<VwSale> OutgoingGrid;
        private List<VwSale> OutgoingList = new();

        private string? myLoc;
        public bool IsVisRole { get; set; } = true;
        private string? myRole;
        public int TotalQty { get; set; }
        public decimal TotalAmt { get; set; }

        //private bool allowPaging = true;
        private decimal FilteredMarginRatio = 0.00M;

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
				//OutgoingList = await myvwSaleService.GetvwSales();
				string StDate1 = "01/" + DateTime.Now.Month.ToString("00") + "/" + DateTime.Now.Year;
				DateTime StDate = DateTime.ParseExact(StDate1, "dd/MM/yyyy", CultureInfo.InvariantCulture);
				DateTime EnDate = DateTime.Now;
				OutgoingList = await myvwSaleService.GetvwSalesDate(StDate.AddDays(0), EnDate.AddDays(1));
                await InvokeAsync(StateHasChanged);
                TotalQty = Convert.ToInt32(OutgoingList.Sum(d => (d.DelQty ?? 0)));
                TotalAmt = Math.Round(OutgoingList.Sum(d => (d.DelTotal ?? 0)), 2);
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

                    if (OutgoingGrid != null)
                    {
                        // ExcelExportProperties exportProps = new ExcelExportProperties();
                        // await OutgoingGrid.ExcelExport(exportProps);
                        //// await OutgoingGrid.ExportToExcelAsync();

                        var columns = OutgoingGrid.Columns.ToList();

                        // Temporarily remove the hidden column from the grid's export
                        //var hiddenColumn = columns.FirstOrDefault(c => c.Field == "DelDispNo");
                        //if (hiddenColumn != null)
                        //{
                        //    columns.Remove(hiddenColumn);
                        //}
                        //var hiddenColumn1 = columns.FirstOrDefault(c => c.Field == "DelDate");
                        //if (hiddenColumn1 != null)
                        //{
                        //    columns.Remove(hiddenColumn1);
                        //}
                        var hiddenColumn2 = columns.FirstOrDefault(c => c.Field == "DelApproved");
                        if (hiddenColumn2 != null)
                        {
                            columns.Remove(hiddenColumn2);
                        }

                        // Set export properties with modified column list
                        ExcelExportProperties exportProps = new ExcelExportProperties
                        {
                            Columns = columns // Only export visible columns
                        };

                        // Export to Excel
                        await OutgoingGrid.ExcelExport(exportProps);

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

        //public void ExcelQueryCellInfoHandler(ExcelQueryCellInfoEventArgs<VwSale> args)
        //{
        //    if (args.Column.Field == "DelDispNo" || args.Column.Field == "DelDate" || args.Column.Field == "DelApproved")
        //    {
        //        args.Cell.Value = "";
        //    }
        //}

        public async Task ValueChangeHandler(RangePickerEventArgs<DateTime?> args)
        {
            DateTime StDate = args.StartDate.Value;
            DateTime EnDate = args.EndDate.Value;
            OutgoingList = await myvwSaleService.GetvwSalesDate(StDate.AddDays(0), EnDate.AddDays(1));
            await InvokeAsync(StateHasChanged);
            TotalQty = Convert.ToInt32(OutgoingList.Sum(d => (d.DelQty ?? 0)));
            TotalAmt = Math.Round(OutgoingList.Sum(d => (d.DelTotal ?? 0)), 2);
            OutgoingGrid.Refresh();
        }
        public void NavigateToPrevious()
        {
            UriHelper.NavigateTo($"blank_pg");
        }

        private async Task OnGridActionComplete(ActionEventArgs<VwSale> args)
        {
            // Get visible records from Grid (filtered + sorted)
            var viewData = await OutgoingGrid.GetCurrentViewRecordsAsync();

            if (viewData != null)
            {
                var filteredList = viewData.Cast<VwSale>().ToList();

                decimal delTotal = filteredList.Sum(x => x.DelTotal ?? 0);
                decimal gross = filteredList.Sum(x => x.Gross ?? 0);

                FilteredMarginRatio = delTotal != 0 ? (gross * 100) / delTotal : 0.00M;

                await InvokeAsync(StateHasChanged); // Refresh UI
            }
        }
    }
}
