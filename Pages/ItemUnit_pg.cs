//using BoldReports.Processing.Objec.Models;
using DigiEquipSys.Models;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;

namespace DigiEquipSys.Pages
{
    public partial class ItemUnit_pg
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

        public string? myRole;
        public string? myLoc;
        public string? vUser;


        WarningPage? Warning;
        string WarningHeaderMessage = "Warning!";
        string WarningContentMessage = "You must select a Unit";

        ConfirmPage? DialogDelete;
        string ConfirmHeaderMessage = "Confirm Delete";
        string ConfirmContentMessage = "Please confirm that you want to Delete this record ";

        private bool SpinnerVisible { get; set; } = false;
        private DialogSettings DialogParams = new() { Width = "650px" };

        private SfGrid<ItemUnit>? ItemUnitGrid;
        public bool IsEdit { get; set; } = false;
        private int unitId;
        protected List<ItemUnit> ItemUnitList = new();
        protected ItemUnit itemunit = new();
        protected AdminInfo admininfo = new();
        protected override async Task OnInitializedAsync()
        {

            try
            {
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                this.SpinnerVisible = true;
                ItemUnitList = await myItemUnit.GetItemUnits();
                //await Task.Delay(1000);
                this.SpinnerVisible = false;
                StateHasChanged();
            }
            catch (Exception ex)
            {
				await JSRuntime.InvokeVoidAsync("alert", ex.Message);
				return;
			}
		}
        public async Task ActionBeginHandler(ActionEventArgs<ItemUnit> Args)
        {
            try
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
                        unitId = 0;
                        unitId = (from bc in ItemUnitList where bc.ItemUnitId == Args.Data.ItemUnitId select bc.ItemUnitId).FirstOrDefault();
                        if (unitId == null || unitId == 0)
                        {
                            await myItemUnit.CreateItemUnit(Args.Data);  //await Http.PostAsJsonAsync("api/GenCountry", Args.Data);
                        }
                        else
                        {
                            WarningContentMessage = "This Unit Information is already exists! It won't be added again.";
                            Warning.OpenDialog();
                        }
                        //await Task.Delay(1000);
                        this.SpinnerVisible = false;
                    }
                    else
                    {
                        if (Args.Data.ItemUnitId != 0)
                        {
                            this.SpinnerVisible = true;
                            unitId = Args.Data.ItemUnitId;
                            var qry = (from bc in ItemUnitList where bc.ItemUnitId == unitId select bc).FirstOrDefault();
                            if (qry != null)
                            {
                                if (qry.ItemUnitId == unitId)
                                {
                                    await myItemUnit.UpdateItemUnit(Args.Data); //await Http.PutAsJsonAsync("api/GenCountry", Args.Data);
                                }
                                else
                                {
                                    WarningContentMessage = "This Unit Information is already exists! You can not overridden.";
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
                    if (Args.Data.ItemUnitId != 0)
                    {
                        unitId = Args.Data.ItemUnitId;
                        DialogDelete.OpenDialog();
                    }
                    else
                    {
                        Warning.OpenDialog();
                    }
                }
            }
			catch (Exception ex)
			{
				await JSRuntime.InvokeVoidAsync("alert", ex.Message);
				return;
			}
		}
    
        protected async Task ConfirmDelete(bool DeleteConfirmed)
        {
            try
            {
                this.SpinnerVisible = true;
                if (DeleteConfirmed)
                {
                    await myItemUnit.DeleteItemUnit(unitId); //await Http.DeleteAsync("api/GenCountry/" + countryId);
                }
                ItemUnitList = await myItemUnit.GetItemUnits(); //await Http.GetFromJsonAsync<List<GenCountry>>("api/GenCountry");
                                                                //await Task.Delay(1000);
                this.SpinnerVisible = false;
                IsEdit = false;
            }
			catch (Exception ex)
			{
				await JSRuntime.InvokeVoidAsync("alert", ex.Message);
				return;
			}
		}


        public async Task ExcelExport()
        {
            try
            {
                this.SpinnerVisible = true;
                await this.ItemUnitGrid.ExportToExcelAsync();
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

    }
}
