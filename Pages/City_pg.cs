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
    public partial class City_pg
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
        string ConfirmContentMessage = "Please confirm that you want to Delete this record " ;

        private SfDialog? DialogAddCity;
        private bool SpinnerVisible { get; set; }=false;

        [Parameter] public string SelectedGenCountry{ get; set; } = "";
        public string cityCode = "";

        public int cityId { get; set; } = 0;
        GenCity addCity = new();
        public bool IsEdit { get; set; } = true;


        private SfGrid<GenCity>? CityGrid;
        protected List<GenCity> CityList = new();
        protected List<GenCountry> CountryList = new();

        SfTextBox? CityDesc;
        SfTextBox? CityCode;


        protected override async Task OnInitializedAsync()
        {
            this.SpinnerVisible = true;
            CountryList = await myGenCountry.GetGenCountryDetails();  //await Http.GetFromJsonAsync<List<GenCountry>>("api/GenCountry");
            CityList = await myGenCity.GetGenCityDetails();  //await Http.GetFromJsonAsync<List<GenCity>>("api/GenCity");
            //await Task.Delay(1000);
            this.SpinnerVisible = false;
        }
        public async Task OnChange(Syncfusion.Blazor.DropDowns.ChangeEventArgs<string, GenCountry> args)
        {
            this.SpinnerVisible = true;
            this.SelectedGenCountry = args.ItemData.CountryCode;
            CityList = await myGenCity.GetGenCityDetails();    //await Http.GetFromJsonAsync<List<GenCity>>("api/GenCity");
            CityList = (from ac in CityList where ac.CityCountryCode == SelectedGenCountry select ac).ToList();
            //await Task.Delay(1000);
            this.SpinnerVisible = false;
            StateHasChanged();
        }

        private async Task AddCity()
        {
            if (SelectedGenCountry == "")
            {
                Warning.OpenDialog();
            }
            else
            {
                IsEdit = false;
                addCity = new GenCity();
                await this.DialogAddCity.ShowAsync();
            }
        }

        private async Task EditCity()
        {
            if (cityId == 0)
            {
                WarningContentMessage = "You must select a City";
                Warning.OpenDialog();
            }
            else
            {
                IsEdit = false;
                this.SpinnerVisible = true;
                addCity = await myGenCity.GetGenCityData(cityId); 
                await this.DialogAddCity.ShowAsync();
                this.SpinnerVisible = false;
            }
        }
        protected async Task CitySave()
        {
            this.SpinnerVisible = true;
            if (addCity.CityId == 0)
            {
                addCity.CityCountryCode = SelectedGenCountry;
                cityCode = (from bc in CityList orderby bc.CityCode descending where bc.CityCountryCode == addCity.CityCountryCode select bc.CityCode).FirstOrDefault();
                if (cityCode == null)
                {
                    addCity.CityCode = "001";
                }
                else
                {
                    if (Convert.ToInt32(cityCode) < 9)
                    {
                        addCity.CityCode = "00" + (Convert.ToInt32(cityCode) + 1).ToString().Trim();
                    }
                    else
                    {
                        if (Convert.ToInt32(cityCode) < 99)
                        {
                            addCity.CityCode = "0" + (Convert.ToInt32(cityCode) + 1).ToString().Trim();
                        }
                        else
                        {
                            addCity.CityCode = (Convert.ToInt32(cityCode) + 1).ToString().Trim();
                        }
                    }
                }
                await myGenCity.AddGenCity(addCity); 
                this.StateHasChanged();
                addCity = new GenCity();
            }
            else
            {
                 await myGenCity.UpdateGenCityDetails(addCity);
                 await this.DialogAddCity.HideAsync();
                 this.StateHasChanged();
            }
            CityList = await myGenCity.GetGenCityDetails(); 
            CityList = (from ac in CityList where ac.CityCountryCode == SelectedGenCountry select ac).ToList();
            cityId = 0;
            this.SpinnerVisible = false;
            IsEdit  = true;
            StateHasChanged();
        }
        public void RowSelectHandler(RowSelectEventArgs<GenCity> args)
        {
            cityId = args.Data.CityId;
            cityCode = args.Data.CityCode;
        }
        public void PrintCity()
        {
            this.CityGrid.Print();
        }

        private async Task DeleteCity()
        {
            if (cityId == 0)
            {
                WarningContentMessage = "You must select a City";
                Warning.OpenDialog();
            }
            else
            {
                ConfirmContentMessage = "Please confirm that you want to Delete this record " + cityCode;
                DialogDelete.OpenDialog();
            }
        }
        protected async Task ConfirmDelete(bool DeleteConfirmed)
        {

            this.SpinnerVisible = true;
            if (DeleteConfirmed)
            {
                await myGenCity.DeleteGenCity(cityId); 
            }
            CityList = await myGenCity.GetGenCityDetails();  
            CityList = (from ac in CityList where ac.CityCountryCode == SelectedGenCountry select ac).ToList();
            this.SpinnerVisible = false;
            StateHasChanged();
        }
        public async Task onOpen(Syncfusion.Blazor.Popups.OpenEventArgs args)
        {
            args.PreventFocus = true;
            await CityCode.FocusAsync();
        }
        private async Task CloseDialog()
        {
            await this.DialogAddCity.HideAsync();
        }

    }
}
