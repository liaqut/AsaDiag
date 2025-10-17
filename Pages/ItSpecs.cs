using DigiEquipSys.Models;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using DigiEquipSys.Shared;
using DigiEquipSys.Interfaces;
using Syncfusion.Blazor.Popups;
using Microsoft.JSInterop;

namespace DigiEquipSys.Pages
{
    public partial class ItSpecs
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
        string WarningContentMessage = "You must select a Customer";

        ConfirmPage? DialogDelete;
        string ConfirmHeaderMessage = "Confirm Delete";
        string ConfirmContentMessage = "Please confirm that you want to Delete this record ";
        private SfGrid<GenScanSpec>? ItSpecsGrid;
        private bool SpinnerVisible { get; set; } = false;

        [Inject]
        public IGenScanSpecService? genscanspecService { get; set; }
        public IEnumerable<GenScanSpec>? genscanspecList;
        SfDialog? DialogGenScanSepecsAddEdit;
        public GenScanSpec genscansepecsaddedit = new();
        private long vDelgenscanId = 0;
        private short vGenScanLength = 0;
        private string? myLoc;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                this.SpinnerVisible = true;
                genscanspecList = await genscanspecService.GetGenScanSpecs();
                this.SpinnerVisible = false;
            }
            catch (Exception ex)
            {
				await JSRuntime.InvokeVoidAsync("alert", ex.Message);
				return;
			}
		}
        public async Task ActionBeginHandler(ActionEventArgs<GenScanSpec> Args)
        {
            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Save))
            {
                this.SpinnerVisible = true;
                if (Args.Action == "Add")
                {
                    vDelgenscanId = 0;
                    vGenScanLength = Convert.ToInt16((from bc in genscanspecList where bc.GenId== Args.Data.GenId select bc.GenScanLength).FirstOrDefault());
                    if (vGenScanLength==0)
                    {
                        GenScanSpec genscansepecsaddedit = new GenScanSpec();
                        genscansepecsaddedit.GenScanLength = Args.Data.GenScanLength;
                        genscansepecsaddedit.GenType = Args.Data.GenType;
                        genscansepecsaddedit.GenListStartFrom = Args.Data.GenListStartFrom;
                        genscansepecsaddedit.GenListLength = Args.Data.GenListLength;
                        genscansepecsaddedit.GenLotStartFrom = Args.Data.GenLotStartFrom;
                        genscansepecsaddedit.GenLotLength = Args.Data.GenLotLength;
                        genscansepecsaddedit.GenExpiryStartFrom = Args.Data.GenExpiryStartFrom;
                        genscansepecsaddedit.GenExpiryLength = Args.Data.GenExpiryLength;
                        genscansepecsaddedit.GenExpiryDir = Args.Data.GenExpiryDir;
                        await genscanspecService.CreateGenScanSpec(genscansepecsaddedit);
                        StateHasChanged();
                    }
                    else
                    {
                        WarningContentMessage = "This Code is already exists! It won't be added again.";
                        Warning.OpenDialog();
                    }
                }
                else
                {
                    if (vDelgenscanId > 0)
                    {
                        var qry = (from bc in genscanspecList where bc.GenId == vDelgenscanId select bc).FirstOrDefault();
                        if (qry != null)
                        {
                            if (qry.GenId == vDelgenscanId)
                            {
                                try
                                {
                                    await genscanspecService.UpdateGenScanSpec(Args.Data);
                                }
                                catch (Exception ex)
                                {
									await JSRuntime.InvokeVoidAsync("alert", ex.Message);
									return;
								}
							}
                            else
                            {
                                WarningContentMessage = "This Code is already exists! You can not overridden.";
                                Warning.OpenDialog();
                            }
                        }
                    }
                    else
                    {
                        WarningContentMessage = "You must select a row";
                        Warning.OpenDialog();
                    }
                }
                ItSpecsGrid.CloseEditAsync();
                this.SpinnerVisible = false;
                StateHasChanged();
            }

            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Delete))
            {
                vDelgenscanId = Args.Data.GenId;
                vGenScanLength = Convert.ToInt16(Args.Data.GenScanLength);

                if (vDelgenscanId > 0)
                {

                    ConfirmContentMessage = "Please confirm that you want to Delete  " + Args.Data.GenScanLength;
                    DialogDelete.OpenDialog();
                }
                else
                {
                    Warning.OpenDialog();
                }
            }
        }

        public void RowSelectHandler(RowSelectEventArgs<GenScanSpec> args)
        {
            vDelgenscanId = args.Data.GenId;
            vGenScanLength = Convert.ToInt16(args.Data.GenScanLength);
        }

        protected async Task ConfirmDelete(bool DeleteConfirmed)
        {
            this.SpinnerVisible = true;
            if (DeleteConfirmed)
            {
                vDelgenscanId = (from qc in genscanspecList where qc.GenScanLength == vGenScanLength select qc.GenId).FirstOrDefault();
                if (vDelgenscanId > 0)
                {
                    await genscanspecService.DeleteGenScanSpec(vDelgenscanId);
                }
            }
            genscanspecList = await genscanspecService.GetGenScanSpecs();
            this.SpinnerVisible = false;
        }
        public async Task ExcelExport()
        {
            try
            {
                this.SpinnerVisible = true;
                await this.ItSpecsGrid.ExportToExcelAsync();
                this.SpinnerVisible = false;
            }
            catch (Exception ex)
            {
				await JSRuntime.InvokeVoidAsync("alert", ex.Message);
				return;
			}
		}

        private void NavigateToPrevious()
        {
             UriHelper.NavigateTo("index");
        }
        protected async Task GenScanSpecsSave()
        {
            this.SpinnerVisible = true;
            try
            {

                if (genscansepecsaddedit.GenScanLength == null || genscansepecsaddedit.GenScanLength == 0)
                {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select a Scan Code from the list";
                    Warning.OpenDialog();
                }
                else
                {
                    var res = await genscanspecService.UpdateGenScanSpec(genscansepecsaddedit);
                    if (res == "ERROR")
                    {
                        WarningHeaderMessage = "Warning!";
                        WarningContentMessage = "Duplicate Entry; You may have duplicated this Specification ";
                        Warning.OpenDialog();
                    }
                }
                genscanspecList = await genscanspecService.GetGenScanSpecs();
                this.SpinnerVisible = false;
                await this.DialogGenScanSepecsAddEdit.HideAsync();
            }
            catch (Exception ex)
            {
				await JSRuntime.InvokeVoidAsync("alert", ex.Message);
				return;
			}
		}
        private async Task CloseDialog()
        {
            await this.DialogGenScanSepecsAddEdit.HideAsync();
        }

    }
}
