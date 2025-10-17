using DigiEquipSys.Models;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;

namespace DigiEquipSys.Pages
{
    public partial class Privilege_pg
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
        string WarningContentMessage = "You must select a Warehouse";
        private bool SpinnerVisible { get; set; } = false;
        private SfGrid<SysPagesControl>? SysPagesControlGrid;
        protected List<AdminInfo> AdminInfoList = new();
        protected List<SystemPage> SystemPagesList = new();
        protected List<SysPagesControl> SysPagesControlList = new();
        public SysPagesControl syspageControl = new();
        public int spid;
        protected override async Task OnInitializedAsync()
        {
            this.SpinnerVisible = true;
            if (vCompType == null || vCompType == "")
            {
                vCompType = "Acc";
            }
            SystemPagesList = await mySystemPagesService.GetSystemPages(vCompType);
            AdminInfoList =await myAdminServic.GetAdminDetails();
            await InvokeAsync(StateHasChanged);
            this.SpinnerVisible = false;
        }
        public async Task OnChange(Syncfusion.Blazor.DropDowns.ChangeEventArgs<string, AdminInfo> args)
        {
            this.SpinnerVisible = true;
            if (vCompType == null || vCompType == "")
            {
                vCompType = "Acc";
            }
            SysPagesControlList = await mySysPagesControl.GetSysPagesControls(args.ItemData.Email,vCompType);   
            var SystemPagesListForLoop = (from im in SystemPagesList where !SysPagesControlList.Any(es => (es.SysPagesControlId == im.PageId)) select im).ToList();
            foreach (var qry in SystemPagesListForLoop)
            {
                syspageControl.SysPagesId = 0;
                syspageControl.SysPagesAuthorized = false;
                syspageControl.SysPagesControlId = qry.PageId;
                syspageControl.SysPagesEmail = args.ItemData.Email;
                await mySysPagesControl.CreateSysPagesControl(syspageControl);
            }
            SysPagesControlList = await mySysPagesControl.GetSysPagesControls(args.ItemData.Email,vCompType);
            this.SpinnerVisible = false;

        }
        public async Task ActionBeginHandler(ActionEventArgs<SysPagesControl> Args)
        {
            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Save))
            {
                this.SpinnerVisible = true;
                if (Args.Data.SysPagesId != 0)
                {
                    spid = Args.Data.SysPagesId;
                    var qry = (from bc in SysPagesControlList where bc.SysPagesId == spid select bc).FirstOrDefault();
                    if (qry != null)
                    {
                        if (qry.SysPagesId == spid)
                        {
                            await mySysPagesControl.UpdateSysPagesControl(Args.Data); 
                        }
                        else
                        {
                            WarningContentMessage = "This Item Information is already exists! You can not overridden.";
                            Warning.OpenDialog();
                        }
                    }
                }
                else
                {
                    WarningContentMessage = "You must select a record";
                    Warning.OpenDialog();
                }
                this.SpinnerVisible = false;
            }
        }

        public void NavigateToPrevious()
        {
            switch (vCompType)
            {
                case "gentrad":
                    UriHelper.NavigateTo($"gentrad");
                    break;
                case "Pos":
                    UriHelper.NavigateTo($"pos");
                    break;
                case "Hrd":
                    UriHelper.NavigateTo($"hrd");
                    break;
                case "Acc":
                    UriHelper.NavigateTo($"blank_pg");
                    break;
                default:
                    UriHelper.NavigateTo($"blank_pg");
                    break;

            }
        }
    }
}
