using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigiEquipSys.Services;
using DigiEquipSys.Models;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using DigiEquipSys.Interfaces;
using Microsoft.JSInterop;

namespace DigiEquipSys.Pages
{
    public partial class Branches_pg
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
        private string? WarningHeaderMessage = "Warning!";
        private string? WarningContentMessage = "You must select a Branch";

        ConfirmPage? DialogDelete;
        private string? ConfirmHeaderMessage = "Confirm Delete";
        private string? ConfirmContentMessage = "Please confirm that you want to Delete this record ";
        public bool IsEdit { get; set; } = true;

        private SfGrid<Branch>? BranchGrid;
        private bool SpinnerVisible { get; set; }=false;
        private int BranchId;
        private string branchCode="";
        private int DelBranchId;
        [Inject] public IBranchService? BranchService { get; set; }
        public IEnumerable<Branch>? BranchList;

        public string[] ColumnItems = new string[] { "Code", "Company Name", "Company Address","City","State","Country", "Post Box No.","Phone","CR Number","VAT Number" };
        private DialogSettings DialogParams = new DialogSettings { Width = "650px" };
        private string? myLoc;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                this.SpinnerVisible = true;
                BranchList = await BranchService.GetBranches();
                //await Task.Delay(1000);
                this.SpinnerVisible = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        public async Task ActionBeginHandler(ActionEventArgs<Branch> Args)
        {
            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Add))
            {
                await BranchGrid.ShowColumnsAsync(ColumnItems);
                IsEdit = true;
            }

            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.BeginEdit))
            {
                await BranchGrid.ShowColumnsAsync(ColumnItems);
                IsEdit = false;
            }

            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Save))
            {
                this.SpinnerVisible = true;
                if (Args.Action == "Add")
                {
                    BranchId = 0;
                    branchCode = (from bc in BranchList where bc.BranchCode == Args.Data.BranchCode select bc.BranchCode).FirstOrDefault();
                    if (branchCode == null)
                    {
                        BranchList = await BranchService.GetBranches();
                        var Qry = (from vBr in BranchList.OrderByDescending(x => x.BranchCode) select vBr).FirstOrDefault();
                        if (Qry == null)
                        {
                            Args.Data.BranchCode = "01" ;
                        }
                        else
                        {
                            if ((Convert.ToInt32(Qry.BranchCode)) < 9)
                            {
                                Args.Data.BranchCode = "0" + (Convert.ToInt32(Qry.BranchCode) + 1).ToString().Trim();
                            }
                            else
                            {
                                Args.Data.BranchCode = (Convert.ToInt32(Qry.BranchCode) + 1).ToString().Trim();
                            }
                        }
                        await BranchService.CreateBranch(Args.Data);
                    }
                    else
                    {
                        WarningContentMessage = "This Branch Code is already exists! It won't be added again.";
                        Warning.OpenDialog();
                    }
                }
                else
                {
                    if (BranchId > 0)
                    {
                        var qry = (from bc in BranchList where bc.BranchCode == Args.Data.BranchCode select bc).FirstOrDefault();
                        if (qry != null)
                        {
                            if (qry.BranchId == BranchId)
                            {
                                 await BranchService.UpdateBranch(Args.Data);
                            }
                            else
                            {
                                WarningContentMessage = "This Branch Code is already exists! You can not overridden.";
                                Warning.OpenDialog();
                            }
                        }
                    }
                    else
                    {
                        WarningContentMessage = "You must select a Branch";
                        Warning.OpenDialog();
                    }
                }
                IsEdit = false;
                await BranchGrid.ShowColumnsAsync(ColumnItems);
                //await Task.Delay(1000);
                this.SpinnerVisible = false;
                StateHasChanged();
            }
            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Delete))
            {
                DelBranchId = Args.Data.BranchId;
                if (DelBranchId > 0)
                {
                    DialogDelete.OpenDialog();
                }
                else
                {
                    Warning.OpenDialog();
                }
            }
        }

        public void RowSelectHandler(RowSelectEventArgs<Branch> args)
        {
            BranchId = args.Data.BranchId;
        }

        protected async Task ConfirmDelete(bool DeleteConfirmed)
        {
            try
            {
                this.SpinnerVisible = true;
                if (DeleteConfirmed)
                {
                    await BranchService.DeleteBranch(DelBranchId);
                }
                BranchList = await BranchService.GetBranches();
                IsEdit = false;
                await BranchGrid.ShowColumnsAsync(ColumnItems);
                await BranchGrid.Refresh();
                //await Task.Delay(1000);
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
            UriHelper.NavigateTo($"blank_pg");
        }
        public async Task ExcelExport()
        {
            try
            {
                this.SpinnerVisible = true;
                await this.BranchGrid.ExportToExcelAsync();
                //await Task.Delay(1000);
                this.SpinnerVisible = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
    }
}
