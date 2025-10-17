using System;
using DigiEquipSys.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using DigiEquipSys.Shared;
using Syncfusion.Blazor.Grids;
using Microsoft.JSInterop;
namespace DigiEquipSys.Pages
{
    public partial class Currency_pg
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

        [Parameter]
        public string? vCompType { get; set; }
        private string? myLoc;

        WarningPage? Warning;
        string WarningHeaderMessage = "Warning!";
        string WarningContentMessage = "You must select a Country";

        ConfirmPage? DialogDelete;
        string ConfirmHeaderMessage = "Confirm Delete";
        string ConfirmContentMessage = "Please confirm that you want to delete this record ";

        private bool SpinnerVisible { get; set; }=false;
        private DialogSettings DialogParams = new() { Width = "650px" };

        private SfGrid<GenCurrency>? CurrencyGrid;
        public bool IsEdit { get; set; } = false;
        private short currencyid;
        protected List<GenCurrency> CurrencyList = new();
        protected GenCurrency gencurrency = new();
        //public string[] ColumnItems = new string[] { "Code" };
        
        protected override async Task OnInitializedAsync()
        {
            try
            {
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                this.SpinnerVisible = true;
                CurrencyList = await myCurrency.GetCurrencies();  
                this.SpinnerVisible = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        public async Task ActionBeginHandler(ActionEventArgs<GenCurrency> Args)
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
                if (Args.Action == "Add")
                {
                    this.SpinnerVisible = true;
                    currencyid = 0;
                    currencyid = (from bc in CurrencyList where bc.CurrId == Args.Data.CurrId select bc.CurrId).FirstOrDefault();
                    if (currencyid == null || currencyid == 0)
                    {
                        await myCurrency.CreateCurrency(Args.Data);  //await Http.PostAsJsonAsync("api/GenCountry", Args.Data);
                    }
                    else
                    {
                        WarningContentMessage = "This Currency Information is already exists! It won't be added again.";
                        Warning.OpenDialog();
                    }
                    //await Task.Delay(1000);
                    this.SpinnerVisible = false;
                }
                else
                {

                    if (Args.Data.CurrId != 0)
                    {
                        this.SpinnerVisible = true;
                        currencyid = Args.Data.CurrId;
                        var qry = (from bc in CurrencyList where bc.CurrId == currencyid select bc).FirstOrDefault();
                        if (qry != null)
                        {
                            await myCurrency.UpdateCurrency(Args.Data); //await Http.PutAsJsonAsync("api/GenCountry", Args.Data);
                        }
                        else
                        {
                            if (qry.CurrId == currencyid)
                            {
                                await myCurrency.UpdateCurrency(Args.Data); //await Http.PutAsJsonAsync("api/GenCountry", Args.Data);
                            }
                            else
                            {
                                WarningContentMessage = "This Currency Information is already exists! You can not overridden.";
                                Warning.OpenDialog();
                            }
                        }
                        //await Task.Delay(1000);
                        this.SpinnerVisible = false;
                    }
                    else
                    {
                        WarningContentMessage = "You must select a record";
                        Warning.OpenDialog();
                    }
                }
                IsEdit = false;
                StateHasChanged();
            }
            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Delete))
            {
                if (Args.Data.CurrId != 0)
                {
                    currencyid = Args.Data.CurrId;
                    DialogDelete.OpenDialog();
                }
                else
                {
                    Warning.OpenDialog();
                }
            }
        }

        protected async Task ConfirmDelete(bool deleteConfirmed)
        {
            this.SpinnerVisible = true;
            if (deleteConfirmed)
            {
                await myCurrency.DeleteCurrency(currencyid); //await Http.DeleteAsync("api/GenCountry/" + countryId);
            }
            CurrencyList = await myCurrency.GetCurrencies(); //await Http.GetFromJsonAsync<List<GenCountry>>("api/GenCountry");
            //await Task.Delay(1000);
            this.SpinnerVisible = false;
            IsEdit = false;
        }


        public async Task ExcelExport()
        {
            try
            {
                this.SpinnerVisible = true;
                await this.CurrencyGrid.ExportToExcelAsync();
                //await Task.Delay(1000);
                this.SpinnerVisible = false;

            }
            catch (Exception)
            {
                throw;
            }

        }
        public void NavigateToPrevious()
        {
            UriHelper.NavigateTo($"blank_pg");
        }
    }
}
