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
    public partial class CatMaster_pg
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
        string WarningContentMessage = "You must select a Category";

        ConfirmPage? DialogDelete;
        string ConfirmHeaderMessage = "Confirm Delete";
        string ConfirmContentMessage = "Please confirm that you want to Delete this record ";

        private SfDialog? DialogAddCat;
        private bool SpinnerVisible { get; set; } = false;

        [Parameter] public string SelectedGroup { get; set; } = "";

        public int catId { get; set; } = 0;
        CategMaster addCat = new();
        public bool IsEdit { get; set; } = true;


        private SfGrid<CategMaster>? catGrid;
        protected List<CategMaster> catList = new();
        protected List<GroupMaster> groupList = new();

        SfTextBox? catDesc;
        SfTextBox? catCode;
        private string catNo { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.SpinnerVisible = true;
            groupList = await mygroupservice.GetGroupMasters();  //await Http.GetFromJsonAsync<List<GenCountry>>("api/GenCountry");
            catList = await mycatservice.GetCategMasters();  //await Http.GetFromJsonAsync<List<GenCity>>("api/GenCity");
            //await Task.Delay(1000);
            this.SpinnerVisible = false;
        }
        public async Task OnChange(Syncfusion.Blazor.DropDowns.ChangeEventArgs<string, GroupMaster> args)
        {
            this.SpinnerVisible = true;
            this.SelectedGroup = args.ItemData.GrpNo;
            catList = await mycatservice.GetCategMasters();
            catList = (from ac in catList where ac.CatGrpNo == SelectedGroup select ac).ToList();
            //await Task.Delay(1000);
            this.SpinnerVisible = false;
            StateHasChanged();
        }

        private async Task AddCategory()
        {
            if (SelectedGroup == "")
            {
                WarningContentMessage = "You must select a Group first";
                Warning.OpenDialog();
            }
            else
            {
                IsEdit = false;
                addCat = new CategMaster();
                await this.DialogAddCat.ShowAsync();
            }
        }

        private async Task EditCategory()
        {
            if (catId == 0)
            {
                WarningContentMessage = "You must select a Category";
                Warning.OpenDialog();
            }
            else
            {
                IsEdit = false;
                this.SpinnerVisible = true;
                addCat = await mycatservice.GetCategMaster(catId);  
                await this.DialogAddCat.ShowAsync();
                this.SpinnerVisible = false;
            }
        }
        protected async Task catSave()
        {
            this.SpinnerVisible = true;
            if (addCat.CatId == 0)
            {
                addCat.CatGrpNo = SelectedGroup;
                catNo = (from bc in catList orderby bc.CatNo descending where bc.CatGrpNo == SelectedGroup select bc.CatNo).FirstOrDefault();
                if (catNo == null)
                {
                    addCat.CatNo = "001";
                }
                else
                {
                    if (Convert.ToInt32(catNo) < 9)
                    {
                        addCat.CatNo = "00" + (Convert.ToInt32(catNo) + 1).ToString().Trim();
                    }
                    else
                    {
                        if (Convert.ToInt32(catNo) < 99)
                        {
                            addCat.CatNo = "0" + (Convert.ToInt32(catNo) + 1).ToString().Trim();
                        }
                        else
                        {
                            addCat.CatNo = (Convert.ToInt32(catNo) + 1).ToString().Trim();
                        }
                    }
                }
                await mycatservice.CreateCategMaster(addCat);
                this.StateHasChanged();
                addCat = new CategMaster();
            }
            else
            {
                await mycatservice.UpdateCategMaster(addCat);
                await this.DialogAddCat.HideAsync();
                this.StateHasChanged();
            }
            catList = await mycatservice.GetCategMasters(); 
            catList = (from ac in catList where ac.CatGrpNo == SelectedGroup select ac).ToList();
            catId = 0;
            //await Task.Delay(1000);
            this.SpinnerVisible = false;
            IsEdit = true;
            StateHasChanged();
        }
        public void RowSelectHandler(RowSelectEventArgs<CategMaster> args)
        {
            catId = args.Data.CatId;
        }
        public void PrintCategory()
        {
            this.catGrid.Print();
        }

        private async Task DeleteCategory()
        {
            if (catId == 0)
            {
                WarningContentMessage = "You must select a Category";
                Warning.OpenDialog();
            }
            else
            {
                ConfirmContentMessage = "Please confirm that you want to Delete this record " ;
                DialogDelete.OpenDialog();
            }
        }
        protected async Task ConfirmDelete(bool DeleteConfirmed)
        {

            this.SpinnerVisible = true;
            catList = await mycatservice.GetCategMasters();
            if (DeleteConfirmed)
            {
                await mycatservice.DeleteCategMaster(catId); //await Http.DeleteAsync("api/GenCity/" + cityId);
            }
            catList = await mycatservice.GetCategMasters();  //await Http.GetFromJsonAsync<List<GenCity>>("api/GenCity");
            catList = (from ac in catList where ac.CatGrpNo == SelectedGroup select ac).ToList();
            //await Task.Delay(1000);
            this.SpinnerVisible = false;
            StateHasChanged();
        }
        public async Task onOpen(Syncfusion.Blazor.Popups.OpenEventArgs args)
        {
            args.PreventFocus = true;
            await catCode.FocusAsync();
        }
        private async Task CloseDialog()
        {
            await this.DialogAddCat.HideAsync();
        }

    }
}
