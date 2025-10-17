using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using DigiEquipSys.Services;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using System.Collections;
using System.Collections.Generic;
namespace DigiEquipSys.Pages
{
    public partial class SsaleHead_pg
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
        [Parameter] public long DelnoteId { get; set; }
        private long detId;
        public bool SpinnerVisible { get; set; } = false;

        WarningPage? Warning;
        string WarningHeaderMessage = "Warning!";
        string WarningContentMessage = "You must select a record";

        ConfirmPage? DialogDelete;
        string ConfirmHeaderMessage = "Confirm Delete";
        string ConfirmContentMessage = "Please confirm that you want to Delete this record ";

        ConfirmPage? DialogDeleteAll;
        string ConfirmHeaderMessageAll = "Confirm Delete All";
        string ConfirmContentMessageAll = "Please confirm that you want to Delete All these records ";


        private SdelHead? sDelvHead = new();
        private SdelDetl? sDelDetl = new();
        private bool enabled { get; set; } = false;

        private SfGrid<VwSale>? OutgoingGrid;
        public List<VwSale>? OutgoingList = new();
        private SfGrid<SdelDetl>? sDelDetlGrid;
        public List<SdelHead>? sDelHeadList = new();
        public List<SdelDetl>? sDelDetlList = new();
        public List<ClientMaster>? ClientMasterList = new();
        protected List<ItemMaster> ItemMasterList = new();
        protected List<GroupMaster>? GroupMasterList { get; set; }
        public string[]? vGrpArray { get; set; }

        private string? myLoc;
        public class DateRange
        {
            public DateTime? selectedRangeFrom { get; set; }
            public DateTime? selectedRangeTo { get; set; }
        }
        private DateRange selectedRange { get; set; } = new DateRange();
        private ItemMaster? sm;
        private SfComboBox<string, ClientMaster> ComboObj;
        private string Custom { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                SpinnerVisible = true;
                ClientMasterList = await myClientMasterService.GetClients();
                ItemMasterList = await myItemMaster.GetItemMasters();
                GroupMasterList = await myGroupMaster.GetGroupMasters();

                if (DelnoteId != 0)
                {
                    enabled = false;

                    sDelvHead = await SdelHeadService.GetSdelHead(DelnoteId);
                    if (sDelvHead != null)
                    {
                        sDelDetlList = await sDelDetlService.GetSDelDetlsBySdelHeadId(DelnoteId);
                        selectedRange.selectedRangeFrom = sDelvHead.SdelDateFrom;
                        selectedRange.selectedRangeTo = sDelvHead.SdelDateTo;
                        OutgoingList = await myvwSaleService.GetvwSalesDate(Convert.ToDateTime(selectedRange.selectedRangeFrom.Value.AddDays(0)), Convert.ToDateTime(selectedRange.selectedRangeTo.Value.AddDays(1)));
                    }
                }
                else
                {
                    enabled = true;
                }
                await InvokeAsync(StateHasChanged);
                SpinnerVisible = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        public void Cancel()
        {
            NavigationManager.NavigateTo("ssale_pg/");
        }

        private async Task ValueChangeGrp(MultiSelectChangeEventArgs<string[]> args)
        {
            vGrpArray = args.Value;
            sDelDetlList = await sDelDetlService.GetSDelDetlsBySdelHeadId(DelnoteId);
            selectedRange.selectedRangeFrom = sDelvHead.SdelDateFrom;
            selectedRange.selectedRangeTo = sDelvHead.SdelDateTo;
            OutgoingList = await myvwSaleService.GetvwSalesDate(Convert.ToDateTime(selectedRange.selectedRangeFrom.Value.AddDays(0)), Convert.ToDateTime(selectedRange.selectedRangeTo.Value.AddDays(1)));
            await SalesbyGrp();
        }

        public async Task SalesbyGrp()
        {
            try
            {
                if (vGrpArray != null && vGrpArray.Length > 0)
                {
                    var filteredList = OutgoingList.Where(v => vGrpArray.Contains(v.ItemGrpCode)).ToList();
                    OutgoingList = filteredList;
                    var delfilteredList = (
                        from x in sDelDetlList
                        join y in ItemMasterList
                        on new { ListNo = x.SdelListNo, ClientCode = x.SdelClientCode }
                        equals new { ListNo = y.ItemListNo, ClientCode = y.ItemClientCode }
                        where vGrpArray.Contains(y.ItemGrpCode)
                        select x
                    ).ToList();
                    sDelDetlList = delfilteredList;
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }


        public async Task ActionBeginHandler(ActionEventArgs<SdelDetl> Args)
        {
            try
            {
                if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Save))
                {
                    if (Args.Action == "Add")
                    {
                        this.SpinnerVisible = true;
                        detId = 0;
                        detId = (from bc in sDelDetlList where bc.SdelDetId == Args.Data.SdelDetId select bc.SdelDetId).FirstOrDefault();
                        if (detId == 0)
                        {
                            await sDelDetlService.CreateSDelDetl(Args.Data);
                        }
                        else
                        {
                            WarningContentMessage = "This Item Information is already exists! It won't be added again.";
                            Warning?.OpenDialog();
                        }
                        this.SpinnerVisible = false;
                    }
                    else
                    {
                        if (Args.Data.SdelDetId != 0)
                        {
                            this.SpinnerVisible = true;
                            detId = Args.Data.SdelDetId;
                            var qry = (from bc in sDelDetlList where bc.SdelDetId == detId select bc).FirstOrDefault();
                            if (qry != null)
                            {
                                if (qry.SdelDetId == detId)
                                {
                                    sm = await myItemMaster.GetItemMaster(Args.Data.SdelListNo, Args.Data.SdelClientCode);
                                    if (sm != null)
                                    {
                                        Args.Data.SdelUprice = sm.ItemSellPrice;
                                    }
                                    else
                                    {
                                        Args.Data.SdelUprice = 0;
                                    }
                                    await sDelDetlService.UpdateSDelDetl(Args.Data);
                                }
                                else
                                {
                                    WarningContentMessage = "This Item Information is already exists! You can not overridden.";
                                    Warning?.OpenDialog();
                                }
                            }
                            this.SpinnerVisible = false;
                        }
                        else
                        {
                            WarningContentMessage = "You must select a record";
                            Warning?.OpenDialog();
                        }
                    }
                    StateHasChanged();
                    await sDelDetlGrid.Refresh();
                }
                if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Delete))
                {
                    if (Args.Data.SdelDetId != 0)
                    {
                        detId = Args.Data.SdelDetId;
                        DialogDelete?.OpenDialog();
                    }
                    else
                    {
                        Warning?.OpenDialog();
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
                    await sDelDetlService.DeleteSDelDetl(detId);
                }
                this.SpinnerVisible = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        protected async Task ConfirmDeleteAll(bool DeleteConfirmed)
        {
            try
            {
                this.SpinnerVisible = true;
                if (DeleteConfirmed)
                {
                    if (sDelvHead != null)
                    {
                        await sDelDetlService.DeleteBySdelHeadId(sDelvHead.SdelId);
                        await SdelHeadService.DeleteSdelHead(sDelvHead.SdelId);
                        NavigationManager.NavigateTo("ssale_pg/");
                    }
                }
                this.SpinnerVisible = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        protected async Task SDelVouDelete()
        {
            ConfirmContentMessage = "Please confirm that you want to Delete" ;
            DialogDeleteAll.OpenDialog();
        }
        protected async Task SDelVouSave()
        {
            try
            {
                await sDelDetlGrid.EndEditAsync();
                this.SpinnerVisible = true;
                enabled = false;
                sDelvHead.SdelApproved = true;
                await SdelHeadService.UpdateSdelHead(sDelvHead);
                foreach (var myDetl in sDelDetlList)
                {
                    await sDelDetlService.UpdateSDelDetl(myDetl);
                }
                this.SpinnerVisible = false;
                string message = "Saved Successfully...";
                await JSRuntime.InvokeVoidAsync("alert", message);
                NavigationManager.NavigateTo($"ssale_pg");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        public async Task ValueChangeHandler(RangePickerEventArgs<DateTime?> args)
        {
            try
            {
                this.SpinnerVisible = true;
                DateTime StDate = args.StartDate.Value;
                DateTime EnDate = args.EndDate.Value;
                OutgoingList = await myvwSaleService.GetvwSalesDate(StDate.AddDays(0), EnDate.AddDays(1));
                StateHasChanged();
                OutgoingGrid.Refresh();

                sDelvHead = await SdelHeadService.GetSdelHead(StDate, EnDate);
                if (sDelvHead != null)
                {
                    await sDelDetlService.DeleteBySdelHeadId(sDelvHead.SdelId);
                    await SdelHeadService.DeleteSdelHead(sDelvHead.SdelId);
                }

                await InvokeAsync(StateHasChanged);

                //sDelvHead = new();
                //sDelHeadList = await SdelHeadService.GetSdelHeads();
                //if (sDelHeadList == null)
                //{
                //    sDelvHead.SdelNo = 1000;
                //}
                //else
                //{
                //    sDelvHead.SdelNo = sDelvHead.SdelNo + 1;
                //}
                //sDelvHead.SdelDispNo = "SS/" + myAccYear.myAccYear + "/" + (Convert.ToString(sDelvHead.SdelNo)).Trim();


                sDelvHead = new();
                sDelvHead.SdelId = 0;
                var nextSdelNo = await SdelHeadService.GetNextSdelNoAsync();
                sDelvHead.SdelNo = (long)nextSdelNo;
                sDelvHead.SdelDispNo = "GRN/" + myAccYear.myAccYear + "/" + nextSdelNo.ToString().Trim();
                sDelvHead.SdelDate = DateTime.Now;
                sDelvHead.SdelDateFrom = StDate;
                sDelvHead.SdelDateTo = EnDate;
                sDelvHead.SdelApproved = false;
                await SdelHeadService.CreateSdelHead(sDelvHead);
                detId = (long)sDelvHead.SdelId;

                sDelDetl = new();
                foreach (var myTrans in OutgoingList)
                {
                    sDelDetl.SdelHeadId = detId;
                    sDelDetl.SdelDate = myTrans.DelDate;
                    sDelDetl.SdelListNo = myTrans.DelListNo;
                    sDelDetl.SdelLotNo = myTrans.DelLotNo;
                    sDelDetl.SdelExpiryDate = myTrans.DelExpiryDate;
                    sDelDetl.SdelQty = myTrans.DelQty;
                    sDelDetl.SdelUprice = myTrans.DelUprice;
                    sDelDetl.SdelStkIdDesc = myTrans.ItemId;
                    sDelDetl.SdelProdCode = myTrans.ItemProdCode;
                    sDelDetl.SdelClientCode = myTrans.DelClientCode;
                    sDelDetl.SdelClientVendCode = myTrans.ClientVendCode;
                    await sDelDetlService.CreateSDelDetl(sDelDetl);
                    sDelDetl = new();
                }
                StateHasChanged();
                sDelDetlList = await sDelDetlService.GetSDelDetlsBySdelHeadId(detId);
                sDelDetlGrid.Refresh();
                this.SpinnerVisible = false;

            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        //public async Task ToolbarClickHandler1(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        //{
        //    if (args.Item.Text == "Excel Export") //Id is combination of Grid's ID and itemname
        //    {
        //        try
        //        {
        //            this.SpinnerVisible = true;
        //            if (OutgoingGrid != null)
        //            {
        //                await OutgoingGrid.ExportToExcelAsync();
        //            }
        //            this.SpinnerVisible = false;
        //        }
        //        catch (Exception ex)
        //        {
        //            await JSRuntime.InvokeVoidAsync("alert", ex.Message);
        //            return;
        //        }
        //    }
        //}
        public async Task ToolbarClickHandler2(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Excel Export") //Id is combination of Grid's ID and itemname
            {
                try
                {
                    //ExcelExportProperties ExportProperties = new ExcelExportProperties();
                    //ExportProperties.IncludeTemplateColumn = true;
                    //await this.sDelDetlGrid.ExportToExcelAsync(ExportProperties);
                    this.SpinnerVisible = true;

                    if (sDelDetlGrid != null)
                    {
                        await sDelDetlGrid.ExportToExcelAsync();
                    }
                    this.SpinnerVisible = false;
                }
                catch (Exception ex)
                {
                    await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                    return;
                }
            }
        }
        private async Task OnFiltering(Syncfusion.Blazor.DropDowns.FilteringEventArgs args)
        {
            try
            {
                Custom = args.Text;
                args.PreventDefaultAction = true;
                var query = new Query().Where(new WhereFilter() { Field = "ClientName", Operator = "contains", value = args.Text, IgnoreCase = true });
                query = !string.IsNullOrEmpty(args.Text) ? query : new Query();
                await ComboObj.FilterAsync(ClientMasterList, query);
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        //public async Task RowUpdatingHandler(RowUpdatingEventArgs<SdelDetl> args)
        //{
        //    try
        //    {
        //        sm = await myStockService.GetStock(args.Data.SdelListNo, args.Data.SdelLotNo, Convert.ToDateTime(args.Data.SdelExpiryDate), args.Data.SdelClientCode);
        //        if (sm != null)
        //        {
        //            args.Data.SdelUprice = sm.ItemSp;
        //            await sDelDetlService.UpdateSDelDetl(args.Data);
        //        }
        //        else
        //        {
        //            args.Data.SdelUprice = 0;
        //            await sDelDetlService.UpdateSDelDetl(args.Data);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await JSRuntime.InvokeVoidAsync("alert", ex.Message);
        //        return;
        //    }
        //}
    }
}
