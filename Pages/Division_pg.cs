using System;
using DigiEquipSys.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using DigiEquipSys.Shared;
namespace DigiEquipSys.Pages
{
    public partial class Division_pg
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
        string WarningContentMessage = "You must select a Company";

        ConfirmPage? DialogDelete;
        string ConfirmHeaderMessage = "Confirm Delete";
        string ConfirmContentMessage = "Please confirm that you want to Delete this record ";

        private SfDialog? DialogAddBranch;
        private bool SpinnerVisible { get; set; } = false;

        [Parameter] public string SelectedCompany { get; set; } = "";
        public string locCode = "";

        public int locId { get; set; } = 0;
        Division addBranch = new();
        public bool IsEdit { get; set; } = true;


        private SfGrid<Division>? BranchGrid;
        protected List<Division> BranchList = new();
        protected List<Branch> CompanyList = new();

        SfTextBox? LocDesc;
        SfTextBox? LocCode;


        protected override async Task OnInitializedAsync()
        {
            this.SpinnerVisible = true;
            CompanyList = await myCompany.GetBranches();  //await Http.GetFromJsonAsync<List<GenCountry>>("api/GenCountry");
            BranchList = await myBranch.GetDivisions();  //await Http.GetFromJsonAsync<List<GenCity>>("api/GenCity");
            //await Task.Delay(1000);
            this.SpinnerVisible = false;
        }
        public async Task OnChange(Syncfusion.Blazor.DropDowns.ChangeEventArgs<string, Branch> args)
        {
            this.SpinnerVisible = true;
            this.SelectedCompany = args.ItemData.BranchCode;
            BranchList = await myBranch.GetDivisions();    //await Http.GetFromJsonAsync<List<GenCity>>("api/GenCity");
            BranchList = (from ac in BranchList where ac.LocBranchCode == SelectedCompany select ac).ToList();
            //await Task.Delay(1000);
            this.SpinnerVisible = false;
            StateHasChanged();
        }

        private async Task AddBranch()
        {
            if (SelectedCompany == "")
            {
                Warning.OpenDialog();
            }
            else
            {
                IsEdit = true;
                addBranch = new Division();
                await this.DialogAddBranch.ShowAsync();
            }
        }

        private async Task EditBranch()
        {
            if (locId == 0)
            {
                WarningContentMessage = "You must select a Company";
                Warning.OpenDialog();
            }
            else
            {
                IsEdit = true;
                this.SpinnerVisible = true;
                addBranch = await myBranch.GetDivision(locId);
                await this.DialogAddBranch.ShowAsync();
                this.SpinnerVisible = false;
            }
        }
        protected async Task BranchSave()
        {
            this.SpinnerVisible = true;
            if (addBranch.LocId == 0)
            {
                addBranch.LocBranchCode = SelectedCompany;
                locCode = (from bc in BranchList orderby bc.LocCode descending where bc.LocBranchCode == addBranch.LocBranchCode select bc.LocCode).FirstOrDefault();
                if (locCode == null)
                {
                    addBranch.LocCode = "001";
                }
                else
                {
                    if (Convert.ToInt32(locCode) < 9)
                    {
                        addBranch.LocCode = "00" + (Convert.ToInt32(locCode) + 1).ToString().Trim();
                    }
                    else
                    {
                        if (Convert.ToInt32(locCode) < 99)
                        {
                            addBranch.LocCode = "0" + (Convert.ToInt32(locCode) + 1).ToString().Trim();
                        }
                        else
                        {
                            addBranch.LocCode = (Convert.ToInt32(locCode) + 1).ToString().Trim();
                        }
                    }
                }
                await myBranch.CreateDivision(addBranch);
                this.StateHasChanged();
                addBranch = new Division();
            }
            else
            {
                await myBranch.UpdateDivision(addBranch);
                await this.DialogAddBranch.HideAsync();
                this.StateHasChanged();
            }
            BranchList = await myBranch.GetDivisions();
            BranchList = (from ac in BranchList where ac.LocBranchCode == SelectedCompany select ac).ToList();
            locId = 0;
            this.SpinnerVisible = false;
            IsEdit = true;
            StateHasChanged();
        }
        public void RowSelectHandler(RowSelectEventArgs<Division> args)
        {
            locId = args.Data.LocId;
            locCode = args.Data.LocCode;
        }
        public void PrintBranch()
        {
            this.BranchGrid.Print();
        }

        private async Task DeleteBranch()
        {
            if (locId == 0)
            {
                WarningContentMessage = "You must select a Company";
                Warning.OpenDialog();
            }
            else
            {
                ConfirmContentMessage = "Please confirm that you want to Delete this record " + locCode;
                DialogDelete.OpenDialog();
            }
        }
        protected async Task ConfirmDelete(bool DeleteConfirmed)
        {

            this.SpinnerVisible = true;
            if (DeleteConfirmed)
            {
                await myBranch.DeleteDivision(locId);
            }
            BranchList = await myBranch.GetDivisions();
            BranchList = (from ac in BranchList where ac.LocBranchCode == SelectedCompany select ac).ToList();
            this.SpinnerVisible = false;
            StateHasChanged();
        }
        public async Task onOpen(Syncfusion.Blazor.Popups.OpenEventArgs args)
        {
            args.PreventFocus = true;
            await LocCode.FocusAsync();
        }
        private async Task CloseDialog()
        {
            await this.DialogAddBranch.HideAsync();
        }

    }
}

