using DigiEquipSys.Models;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using DigiEquipSys.Shared;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.Navigations;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Calendars;

namespace DigiEquipSys.Pages
{
    public partial class CommCharge_pg
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

        private string? myLoc;
        WarningPage? Warning;
        string WarningHeaderMessage = "Warning!";
        string WarningContentMessage = "You must select an Item";

        ConfirmPage? DialogDelete;
        string ConfirmHeaderMessage = "Confirm Delete";
        string ConfirmContentMessage = "Please confirm that you want to Delete this record ";

        private bool SpinnerVisible { get; set; } = false;
        private DialogSettings DialogParams = new() { Width = "650px" };
        protected List<CommCharge> CommChargeList = new();
        protected CommCharge commcharge = new();
        private SfGrid<CommCharge>? CommChargeGrid;
        private long commId;

        private SfDialog? DialogAddCommCharge;
        private List<ItemModel> Toolbaritems = new();
        private int sw { get; set; } = 0;
        private string? myRole;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                myRole = await sessionStorage.GetItemAsync<string>("adminRo");
                this.SpinnerVisible = true;
                CommChargeList = await myCommCharge.GetCommCharges();  // await Http.GetFromJsonAsync<List<GenCountry>>("api/GenCountry");

                //await Task.Delay(1000);
                this.SpinnerVisible = false;
                Toolbaritems.Add(new ItemModel() { Text = "Add", TooltipText = "Add new", PrefixIcon = "e-add" });
                Toolbaritems.Add(new ItemModel() { Text = "Edit", TooltipText = "Edit", PrefixIcon = "e-edit" });
                Toolbaritems.Add(new ItemModel() { Text = "Delete", TooltipText = "Delete a Record", PrefixIcon = "e-delete" });
                Toolbaritems.Add(new ItemModel() { Text = "Export", TooltipText = "Export to Excel", PrefixIcon = "e-export" });
                await InvokeAsync(StateHasChanged);
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
                    if (CommChargeGrid != null)
                    {
                        await CommChargeGrid.ExportToExcelAsync();
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
                    if (commId != 0)
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
                commcharge = new CommCharge();
                await this.DialogAddCommCharge.ShowAsync();
            }

            if (args.Item.Text == "Edit")
            {
                if (commId == 0)
                {
                    WarningContentMessage = "You must select an Item";
                    Warning.OpenDialog();
                }
                else
                {
                    this.SpinnerVisible = true;
                    commcharge = await myCommCharge.GetCommCharge(commId);
                    await this.DialogAddCommCharge.ShowAsync();
                    this.SpinnerVisible = false;
                }
            }
        }

        private async Task CloseDialog()
        {
            await this.DialogAddCommCharge.HideAsync();
        }

        protected async Task CommChargeSave()
        {
            try
            {
                sw = 0;
                if (commcharge.CommDesc == null || commcharge.CommDesc == "")
                {
                    WarningContentMessage = "Fill Item Description/Name";
                    Warning.OpenDialog();
                    sw = 1;
                }
                if (commcharge.CommAmt == 0 || commcharge.CommAmt == null)
                {
                    WarningContentMessage = "Enter Commission Amount...";
                    Warning.OpenDialog();
                    sw = 1;
                }
                if (sw == 0)
                {
                    this.SpinnerVisible = true;
                    if (commcharge.CommId == 0)
                    {
                        await myCommCharge.CreateCommCharge(commcharge);
                        this.StateHasChanged();
                        commcharge = new CommCharge();
                    }
                    else
                    {
                        await myCommCharge.UpdateCommCharge(commcharge);
                        await this.DialogAddCommCharge.HideAsync();
                        this.StateHasChanged();
                    }
                    CommChargeList = await myCommCharge.GetCommCharges();
                    this.SpinnerVisible = false;
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        public void RowSelectHandler(RowSelectEventArgs<CommCharge> args)
        {
            commId = args.Data.CommId;
        }

        protected async Task ConfirmDelete(bool DeleteConfirmed)
        {
            this.SpinnerVisible = true;
            if (DeleteConfirmed)
            {
                await myCommCharge.DeleteCommCharge(commId); //await Http.DeleteAsync("api/GenCountry/" + countryId);
            }
            CommChargeList = await myCommCharge.GetCommCharges(); //await Http.GetFromJsonAsync<List<GenCountry>>("api/GenCountry");
            this.SpinnerVisible = false;
        }
        public async Task ValueChangeHandler(RangePickerEventArgs<DateTime?> args)
        {
            DateTime StDate = args.StartDate.Value;
            DateTime EnDate = args.EndDate.Value;
            CommChargeList = await myCommCharge.GetCommChargeDate(StDate.AddDays(0), EnDate.AddDays(1));
            await InvokeAsync(StateHasChanged);
            CommChargeGrid.Refresh();
        }
        public void NavigateToPrevious()
        {
            UriHelper.NavigateTo($"blank_pg");
        }
    }
}
