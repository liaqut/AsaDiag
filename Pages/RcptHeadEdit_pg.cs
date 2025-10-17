using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Grids.Internal;
using Syncfusion.Blazor.Inputs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Reflection.Emit;

namespace DigiEquipSys.Pages
{
    public partial class RcptHeadEdit_pg
    {
        [CascadingParameter]
        public EventCallback notify { get; set; }
        private string? myRole;
        private string? myLoc;
        private string? myUser;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                myUser = await sessionStorage.GetItemAsync<string>("adminEmail");
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                await notify.InvokeAsync();
                StateHasChanged();
            }
        }

        [Parameter]
        public string? vCompType { get; set; }
        public string vEdit = "Yes";

        WarningPage? Warning;
        string WarningHeaderMessage = "";
        string WarningContentMessage = "";
        public bool SpinnerVisible { get; set; } = false;
        [Parameter] public long RcptvouId { get; set; }
        public long RcptSerNo { get; set; }
        public int? VendId { get; set; }
        public long PohId { get; set; }
        public string? rcvWhCode { get; set; }
        public long? RcvPoNo { get; set; }
        private SfGrid<RcptDetail>? RcptDetGrid;

        [Inject]
        public IRcptHeadService? RcptHeadService { get; set; }
        public IEnumerable<RcptHead>? RcptVouList;
        public RcptHead rcptvouaddedit = new();

        [Inject]
        public IRcptDetailService? RcptDetailService { get; set; }
        protected List<RcptDetail> rcptvoudetails = new();
        private RcptDetail rcpt =new RcptDetail() ;
        private ItemMaster? im;
        private long? vRdId;
        private double? TotrcptQty;

        public List<GroupMaster>? ItemGroupList = new();
        public List<CategMaster>? ItemCatList = new();

        protected List<ItemMaster> ItemMasterList = new();
        protected List<ItemUnit>? ItemUnitList = new();

        public List<GenScanSpec> genScanSpecList = new();
        //protected Stock currStock=new();
        private GenScanSpec qryspec { get; set; }
        private bool isButtonDisabled = false;
        public long? RcptNo { get; set; }
        private int mysw = 0;
        public string? myLocShort { get; set; }
        public int iSw = 0;
        //private bool isNewRcpt = true;
        private bool isChkList = true;
        private bool vEnable { get; set; } = true;
        //private bool vbuttonEnable { get; set; } = false;
        //protected List<PoHead> PohList = new();
        protected List<SupplierMaster> SupplierMasterList = new();
        protected List<ClientMaster> ClientMasterList = new();
        private SfComboBox<string, ClientMaster> ComboObj;
        private SfComboBox<string, SupplierMaster> ComboObj2;

        private string Custom { get; set; }
        private bool showEditButton = true;
        public bool fillsw1 = false;
        public bool IsVisRole { get; set; } = true;
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
        //private Dictionary<RcptDetail, List<PoHead>> RowPoCache = new();
        protected override async Task OnInitializedAsync()
        {

            try
            {
                this.SpinnerVisible = true;
                //isButtonDisabled = true;
                rcptvouaddedit.RhDate = DateTime.Now;
                rcptvouaddedit.RhDateAltered = DateTime.Now;
                rcptvouaddedit.RhUser = myUser;
                SupplierMasterList = await mySupplierService.GetSuppliers();
                ClientMasterList = await myClientService.GetClients();
                genScanSpecList = await myScanSpecService.GetGenScanSpecs();
                if (RcptvouId != 0)
                {
                    rcptvouaddedit = await RcptHeadService.GetRcptHead(RcptvouId);
                    RcptSerNo = rcptvouaddedit.RhId;
                    rcptvoudetails = await RcptDetailService.GetRcptDetails(RcptSerNo);
                    TotalItems = rcptvoudetails.Count();
                    TotalAmount = Math.Round(rcptvoudetails.Sum(d => d.TotalPrice), 2);

                    //isButtonDisabled = false;
                    isChkList = false;
                    if (rcptvouaddedit.RhApproved == true)
                    {
                        vEnable = false;
                        isButtonDisabled = true;
                    }
                    else
                    {
                        isButtonDisabled = false;
                    }
                }
                ItemMasterList = await myItemMaster.GetItemMasters();
                ItemUnitList = await myUnitService.GetItemUnits();
                ItemGroupList = await myGrpService.GetGroupMasters();
                ItemCatList = await myCatService.GetCategMasters();
                //PohList = await myPoheadService.GetPoHeads();

                myRole = await sessionStorage.GetItemAsync<string>("adminRo");
                if (myRole == "01" || myRole == "02" || myRole == "03")
                {
                    IsVisRole = true;
                }
                else
                {
                    IsVisRole = false;
                }
                await InvokeAsync(StateHasChanged);
                this.SpinnerVisible = false;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        public IEditorSettings myEditParams = new NumericEditCellParams
        {
            Params = new NumericTextBoxModel<object>() { ShowClearButton = true, ShowSpinButton = false }
        };
        //private async Task UpdatePohDispNo(RcptDetail poh)
        //{
        //    try
        //    {
        //        await RcptDetailService.UpdateRcptDetail(poh);
        //        await InvokeAsync(StateHasChanged);
        //        await RcptDetGrid.Refresh();
        //    }
        //    catch (Exception ex)
        //    {
        //        await JSRuntime.InvokeVoidAsync("alert", ex.Message);
        //        return;
        //    }
        //}
        //private void OnPoSelected(RcptDetail row, string selectedPo)
        //{
        //    row.RdPohDispNo = selectedPo;
        //}
        //private async Task UpdatePurchaseOrderDropdown(RcptDetail order)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(order?.RdSuppCode))
        //        {
        //            var result = await myPoheadService.GetPoHeads(order.RdSuppCode);
        //            order.PohList = result ?? new List<PoHead>();
        //        }
        //        else
        //        {
        //            order.PohList = new();
        //        }
        //        await Task.Delay(100);
        //        await InvokeAsync(StateHasChanged);
        //        await RcptDetGrid.Refresh();
        //    }
        //    catch (Exception ex)
        //    {
        //        await JSRuntime.InvokeVoidAsync("alert", ex.Message);
        //        return;
        //    }
        //}


        public async Task Cancel()
        {
            if (isButtonDisabled == false)
            {
                await RcptVouSave();
            }
            if (fillsw1 == false)
            {
                NavigationManager.NavigateTo("rcptedit_pg/");
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("alert", "few Mandatory fields are empty...");
                return;
            }
            //NavigationManager.NavigateTo($"rcptedit_pg");
        }

        public async Task CheckList()
        {
            if (isButtonDisabled == false)
            {
                await RcptVouSave();
            }
            if (fillsw1 == false)
            {
                NavigationManager.NavigateTo("checkList_pg/" + RcptvouId + "/" + vEdit);
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("alert", "few Mandatory fields are empty...");
                return;
            }
            //NavigationManager.NavigateTo("checkList_pg/" + RcptvouId + "/" + vEdit);
        }

        public async Task RowUpdatingHandler(RowUpdatingEventArgs<RcptDetail> args)
        {
            try
            {
                if (isButtonDisabled == false)
                {
                    im = await myItemMaster.GetItemMaster(args.Data.RdListNo, args.Data.RdClientCode);
                    if (im != null)
                    {
                        args.Data.RdUp = Convert.ToDecimal(im.ItemCostPrice);
                        args.Data.RdStkIdGrp = im.ItemGrpCode;
                        args.Data.RdStkIdCat = im.ItemCatCode;
                    }
                    else
                    {
                        args.Cancel = true;
                        await JSRuntime.InvokeVoidAsync("alert", "Item is not in the Customer List..");
                        return;
                    }
                }
                else
                {
                    args.Cancel = true;
                    await JSRuntime.InvokeVoidAsync("alert", "Approved Voucher..You Can't Add/Edit..");
                    return;
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        protected async Task RcptVouSave()
        {
            await RcptDetGrid.EndEditAsync();
            this.SpinnerVisible = true;
            fillsw1 = false;

            if (rcptvoudetails != null)
            {
                if (RcptvouId != 0)
                {
                    try
                    {
                        await RcptHeadService.UpdateRcptHead(rcptvouaddedit);
                        foreach (var individualEntry in rcptvoudetails)
                        {
                            if (individualEntry.RdLotNo == null || individualEntry.RdExpiryDate == null)
                            {
                                fillsw1 = true;
                                break;
                            }
                            if (individualEntry.RdId > 0)
                            {
                                await RcptDetailService.UpdateRcptDetail(individualEntry);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                        return;
                    }
                }
            }
            else
            {
                WarningContentMessage = "Nothing to Save";
                Warning.OpenDialog();
            }
            //isButtonDisabled = false;
            this.SpinnerVisible = false;
            //string message = "Saved Successfully...";
            //await JSRuntime.InvokeVoidAsync("alert", message);
            //NavigationManager.NavigateTo($"rcptedit_pg");

        }
        private async Task OnFiltering(Syncfusion.Blazor.DropDowns.FilteringEventArgs args)
        {
            Custom = args.Text;
            args.PreventDefaultAction = true;
            var query = new Query().Where(new WhereFilter() { Field = "ClientName", Operator = "contains", value = args.Text, IgnoreCase = true });
            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();
            await ComboObj.FilterAsync(ClientMasterList, query);
        }
        private async Task OnFiltering2(Syncfusion.Blazor.DropDowns.FilteringEventArgs args)
        {
            Custom = args.Text;
            args.PreventDefaultAction = true;
            var query = new Query().Where(new WhereFilter() { Field = "SuppName", Operator = "contains", value = args.Text, IgnoreCase = true });
            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();
            await ComboObj2.FilterAsync(SupplierMasterList, query);
        }
    }
}