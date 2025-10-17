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
    public partial class AddUser_pg
    {
        public bool adduser=true;

        [CascadingParameter]
        public EventCallback notify { get; set; }

       protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await notify.InvokeAsync();
                if (adduser == false)
                {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Reached your Maximum user limits. Please contact the vendor for additional user(s)";
                    Warning.OpenDialog();
                }
            }
        }

        public bool IsEdit { get; set; } = true;

        WarningPage? Warning;
        private string? WarningHeaderMessage = "Warning!";
        private string? WarningContentMessage = "Reached your Maximum user limits. Please contact the vendor for additional user(s)";

        ConfirmPage? DialogDelete;
        private string? ConfirmHeaderMessage = "Confirm Delete";
        private string? ConfirmContentMessage = "Please confirm that you want to Delete this record ";
        
        private SfGrid<AdminInfo>? UserGrid;
        private bool SpinnerVisible { get; set; } = false;
        private int userId;
        private string userEmail = "";
        private int DelUserId;
        [Inject] public IAdminService? AdminService { get; set; }
        public IEnumerable<AdminInfo>? UserList;
        [Inject] public ISysPagesControlService sysPagesService { get; set; }

        [Inject] public IRoleService? RoleService { get; set; }
        public IEnumerable<RoleInfo>? RoleList;

        //public string[] ColumnItems = new string[] { "Id", "Name", "Email", "Password" };
        private DialogSettings DialogParams = new DialogSettings { Width = "650px" };
        private string? myLoc;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                this.SpinnerVisible = true;
                UserList = await AdminService.GetAdminDetails();
                RoleList = await RoleService.GetRoleDetails();
                if (UserList.Count() >= 4)
                {
                    adduser = false;
                }
                else
                {
                    adduser = true;

                }
                this.SpinnerVisible = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }

        }

        public async Task ActionBeginHandler(ActionEventArgs<AdminInfo> Args)
        {
            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Add))
            {
                //Warning.CloseDialog();
                UserList = await AdminService.GetAdminDetails();
                if (UserList.Count() >= 4)
                {
                    adduser = false;
                }
                else
                {
                    adduser = true;

                }
                //await UserGrid.ShowColumnsAsync(ColumnItems);
                IsEdit = true;
                if (adduser==false)
                {
                    try
                    {
                        Args.Cancel = true;
                        UserGrid.EditSettings.AllowAdding = false;
                        WarningContentMessage = "You are exceeding the Users Count Limit";
                        Warning.OpenDialog();
                        StateHasChanged();
                    }
                    catch (Exception ex)
                    {
                        await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                        return;
                    }
                }
                else
                {
                    UserGrid.EditSettings.AllowAdding = true;
                }
            }

            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.BeginEdit))
            {
                //await UserGrid.ShowColumnsAsync(ColumnItems);
                IsEdit = true;
            }
            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Save))
            {
                this.SpinnerVisible = true;
                if (Args.Action == "Add")
                {
                    userId = 0;
                    userEmail = (from bc in UserList where bc.Id == Args.Data.Id select bc.Email).FirstOrDefault();
                    if (userEmail == null)
                    {
                        await AdminService.CreateAdminInfo(Args.Data);
                    }
                    else
                    {
                        WarningContentMessage = "This User is already exists! ";
                        Warning.OpenDialog();
                    }
                }
                else
                {
                    if (userId > 0)
                    {
                        var qry = (from bc in UserList where bc.Email == Args.Data.Email select bc).FirstOrDefault();
                        if (qry != null)
                        {
                            if (qry.Id == userId)
                            {
                                await AdminService.UpdateAdminInfo(Args.Data);
                            }
                            else
                            {
                                WarningContentMessage = "This User is already exists! You can not overridden.";
                                Warning.OpenDialog();
                            }
                        }
                    }
                    else
                    {
                        WarningContentMessage = "You must select a User";
                        Warning.OpenDialog();
                    }
                }
                IsEdit = false;
                //await UserGrid.ShowColumnsAsync(ColumnItems);
                this.SpinnerVisible = false;
                StateHasChanged();
                await UserGrid.Refresh();
            }
            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Delete))
            {
                DelUserId = Args.Data.Id;
                userEmail = Args.Data.Email;
                if (DelUserId > 0)
                {
                    DialogDelete.OpenDialog();
                }
                else
                {
                    Warning.OpenDialog();
                }
            }
        }

        public void RowSelectHandler(RowSelectEventArgs<AdminInfo> args)
        {
            userId = args.Data.Id;
        }

        protected async Task ConfirmDelete(bool DeleteConfirmed)
        {
            try
            {
                this.SpinnerVisible = true;
                if (DeleteConfirmed)
                {
                    await sysPagesService.DeleteSysPagesControl(userEmail);
                    await AdminService.DeleteAdminInfo(DelUserId);
                }
                //await UserGrid.ShowColumnsAsync(ColumnItems);
                await UserGrid.Refresh();
                UserList = await AdminService.GetAdminDetails();
                if (UserList.Count() >= 4)
                {
                    adduser = false;
                }
                else
                {
                    adduser = true;

                }
                IsEdit = false;
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
                await this.UserGrid.ExportToExcelAsync();
                //await Task.Delay(1000);
                this.SpinnerVisible = false;
            }
            catch (Exception ex )
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
    }
}
