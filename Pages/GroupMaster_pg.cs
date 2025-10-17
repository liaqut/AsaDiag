using System;
using DigiEquipSys.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using DigiEquipSys.Shared;
using Microsoft.JSInterop;

namespace DigiEquipSys.Pages
{
    public partial class GroupMaster_pg
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

        WarningPage? Warning;
        string WarningHeaderMessage = "Warning!";
        string WarningContentMessage = "You must select a Group";

        ConfirmPage? DialogDelete;
        string ConfirmHeaderMessage = "Confirm Delete";
        string ConfirmContentMessage = "Please confirm that you want to Delete this record ";

        private bool SpinnerVisible { get; set; } = false;
        private DialogSettings DialogParams = new DialogSettings { Width = "650px" };

        private SfGrid<GroupMaster>? GroupMasterGrid;
        public bool IsEdit { get; set; } = false;
        private int groupId;
        private string grpno;
        protected List<GroupMaster> GroupList = new();
        protected GroupMaster groupmaster = new();
        public string[] ColumnItems = new string[] { "Group No." };
        private string? myLoc;
        private string vGrpNo = "";
        protected override async Task OnInitializedAsync()
        {
            try
            {
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                this.SpinnerVisible = true;
                GroupList = await myGroupMaster.GetGroupMasters();  // await Http.GetFromJsonAsync<List<GenCountry>>("api/GenCountry");
                //await Task.Delay(1000);
                this.SpinnerVisible = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        public async Task ActionBeginHandler(ActionEventArgs<GroupMaster> Args)
        {
            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Add))
            {
                await GroupMasterGrid.HideColumnsAsync(ColumnItems);
                IsEdit = true;
            }

            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.BeforeBeginEdit))
            {
                await GroupMasterGrid.HideColumnsAsync(ColumnItems);
                IsEdit = false;
            }

            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Cancel))
            {
                await GroupMasterGrid.ShowColumnsAsync(ColumnItems);
                IsEdit = false;
            }

            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Save))
            {
                if (Args.Action == "Add")
                {
                    this.SpinnerVisible = true;
                    var Qry = (from vBr in GroupList.OrderByDescending(x => x.GrpNo) select vBr).FirstOrDefault();
                    if (Qry == null)
                    {
                        Args.Data.GrpNo = "01";
                    }
                    else
                    {
                        if (Convert.ToInt32(Qry.GrpNo) < 9)
                        {
                            Args.Data.GrpNo = "0" + (Convert.ToInt32(Qry.GrpNo) + 1).ToString().Trim();

                        }
                        else
                        {
                            Args.Data.GrpNo = (Convert.ToInt32(Qry.GrpNo) + 1).ToString().Trim();
                        }
                    }
                    GroupMaster groupmaster = new GroupMaster();
                    groupmaster.GrpNo = Args.Data.GrpNo;
                    groupmaster.GrpDesc = Args.Data.GrpDesc;
                    groupmaster.GrpShortDesc = Args.Data.GrpShortDesc;
                    await myGroupMaster.CreateGroupMaster(groupmaster);  //await Http.PostAsJsonAsync("api/GenCountry", Args.Data);
                    this.SpinnerVisible = false;
                    groupId = groupmaster.GrpId;
                    StateHasChanged();
                    await GroupMasterGrid.Refresh();
                }
                else
                {
                    if (Args.Data.GrpId != 0)
                    {
                        this.SpinnerVisible = true;
                        groupId = Args.Data.GrpId;
                        var qry = (from bc in GroupList where bc.GrpId == groupId select bc).FirstOrDefault();
                        if (qry != null)
                        {
                            if (qry.GrpId == groupId)
                            {
                                await myGroupMaster.UpdateGroupMaster(Args.Data); //await Http.PutAsJsonAsync("api/GenCountry", Args.Data);
                            }
                            else
                            {
                                WarningContentMessage = "This Group Information is already exists! You can not overridden.";
                                Warning.OpenDialog();
                            }
                        }
                        this.SpinnerVisible = false;
                    }
                    else
                    {
                        WarningContentMessage = "You must select a record";
                        Warning.OpenDialog();
                    }
                }
                IsEdit = false;
                await GroupMasterGrid.ShowColumnsAsync(ColumnItems);
                StateHasChanged();
            }

            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Delete))
            {
                if (Args.Data.GrpNo != "")
                {
                    groupId = Args.Data.GrpId;
                    vGrpNo = Args.Data.GrpNo;
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
            GroupList = await myGroupMaster.GetGroupMasters();
            if (DeleteConfirmed)
            {
                groupId = (from qc in GroupList where qc.GrpNo == vGrpNo select qc.GrpId).FirstOrDefault();
                if (groupId > 0)
                {
                    await myGroupMaster.DeleteGroupMaster(groupId);
                }
            }
            GroupList = await myGroupMaster.GetGroupMasters(); 
            this.SpinnerVisible = false;
            await GroupMasterGrid.ShowColumnsAsync(ColumnItems);
            IsEdit = false;
        }


        public async Task ExcelExport()
        {
            try
            {
                this.SpinnerVisible = true;
                await this.GroupMasterGrid.ExportToExcelAsync();
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
