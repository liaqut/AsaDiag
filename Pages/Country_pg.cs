using System;
using DigiEquipSys.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using DigiEquipSys.Shared;
using Syncfusion.Blazor.Grids;
using Microsoft.JSInterop;

namespace DigiEquipSys.Pages
{
    public partial class Country_pg
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
        string WarningContentMessage = "You must select a Country";

        ConfirmPage? DialogDelete;
        string ConfirmHeaderMessage = "Confirm Delete";
        string ConfirmContentMessage = "Please confirm that you want to Delete this record ";

        private bool SpinnerVisible { get; set; }=false;
        private DialogSettings DialogParams = new() { Width = "650px" };

        private SfGrid<GenCountry>? CountryGrid;
        public bool IsEdit { get; set; } = false;
        private int countryId;
        protected List<GenCountry> CountryList = new();
        protected GenCountry country = new();
        //public string[] ColumnItems = new string[] { "Code" };

        protected override async Task OnInitializedAsync()
        {
            this.SpinnerVisible = true;
            CountryList = await myGenCountry.GetGenCountryDetails();  // await Http.GetFromJsonAsync<List<GenCountry>>("api/GenCountry");
            //await Task.Delay(1000);
            this.SpinnerVisible = false;
        }
        public async Task ActionBeginHandler(ActionEventArgs<GenCountry> Args)
        {
            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Add))
            {
                //await UserInfoGrid.HideColumnsAsync(ColumnItems);
                IsEdit = true;
                //Args.PreventRender = false;
                //CountryGrid.EditSettings.Mode = EditMode.Normal;
            }

            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.BeforeBeginEdit))
            {
                //Args.PreventRender = false;
                //CountryGrid.EditSettings.Mode = EditMode.Dialog;
                //Args.PreventRender = false;
                IsEdit = false;
            }

            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Cancel))
            {
                //await UserInfoGrid.ShowColumnsAsync(ColumnItems);
                IsEdit = false;
            }

            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Save))
            {
                this.SpinnerVisible = true;
                if (Args.Action == "Add")
                {
                    countryId = 0;
                    countryId = (from bc in CountryList where bc.CountryCode == Args.Data.CountryCode select bc.CountryId).FirstOrDefault();
                    if (countryId == null || countryId==0)
                    {
                        await myGenCountry.AddGenCountry(Args.Data);  //await Http.PostAsJsonAsync("api/GenCountry", Args.Data);
                    }
                    else
                    {
                        WarningContentMessage = "This Country Information is already exists! It won't be added again.";
                        Warning.OpenDialog();
                    }
                }
                else
                {

                    if (Args.Data.CountryId != 0)
                    {
                        countryId = Args.Data.CountryId; 
                        var qry = (from bc in CountryList where bc.CountryId == countryId select bc).FirstOrDefault();
                        if (qry != null)
                        {
                            if (qry.CountryId == countryId)
                            {
                                await myGenCountry.UpdateGenCountryDetails(Args.Data); //await Http.PutAsJsonAsync("api/GenCountry", Args.Data);
                            }
                            else
                            {
                                WarningContentMessage = "This Country Information is already exists! You can not overridden.";
                                Warning.OpenDialog();
                            }
                        }
                    }
                    else
                    {
                        WarningContentMessage = "You must select a record";
                        Warning.OpenDialog();
                    }
                }
                IsEdit = false;
                //await Task.Delay(1000);
                this.SpinnerVisible = false;
                StateHasChanged();

            }
            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Delete))
            {
                if (Args.Data.CountryId != 0)
                {
                    countryId = Args.Data.CountryId;
                    ConfirmContentMessage = "Please confirm that you want to Delete  " + Args.Data.CountryCode;
                    DialogDelete.OpenDialog();
                }
                else
                {
                    Warning.OpenDialog();
                }
            }
        }

        protected async Task ConfirmDelete(bool DeleteConfirmed)
        {
            this.SpinnerVisible = true;
            if (DeleteConfirmed)
            {
                await myGenCountry.DeleteGenCountry(countryId); //await Http.DeleteAsync("api/GenCountry/" + countryId);
            }
            CountryList = await myGenCountry.GetGenCountryDetails(); //await Http.GetFromJsonAsync<List<GenCountry>>("api/GenCountry");
            //await Task.Delay(1000);
            this.SpinnerVisible = false;
            IsEdit = false;
        }


        public async Task ExcelExport()
        {
            try
            {
                this.SpinnerVisible = true;
                await this.CountryGrid.ExportToExcelAsync();
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
