using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;
using System.Numerics;
using System.Text.Json;

namespace DigiEquipSys.Pages
{
    public partial class Supplier_pg
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
        string WarningContentMessage = "You must select a Supplier";

        ConfirmPage? DialogDelete;
        string ConfirmHeaderMessage = "Confirm Delete";
        string ConfirmContentMessage = "Please confirm that you want to Delete this record ";

        [Inject]
        public IGenCountryService? countryService { get; set; }
        public IEnumerable<GenCountry>? countryList;

        [Inject]
        public IGenCityService? cityService { get; set; }
        public IEnumerable<GenCity>? cityList;

        private SfGrid<SupplierMaster>? SupplierGrid;
        private long DetailId;
        private string detailCode = "";
        private long DelDetailId;
        public bool SpinnerVisible { get; set; }=false;
        [Parameter] public string vhref { get; set; }
        public string MyRacc { get; set; } = "02";
        //public string[] ColumnItems = new string[] { "Code" };
        public SupplierMaster suppliersaddedit = new();
        [Inject] public ISupplierService? SupplierService { get; set; }
        public IEnumerable<SupplierMaster>? SupplierList;
        SfDialog? DialogSuppliersAddEdit;
        private int vSuppId = 0;
        private string vSuppCode = "";
        private string? myLoc;
        public bool Initial { get; set; } = true;
        private List<Object> ToolbarItems { get; set; }
        bool isDelete = false;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                this.SpinnerVisible = true;
                SupplierList = await SupplierService.GetSuppliers();
                countryList = await countryService.GetGenCountryDetails();
                this.SpinnerVisible = false;
                ToolbarItems = new List<object>()
                {
                    "Add", "Delete", "Edit", "Cancel", "Update","Print",
                    new ItemModel() { Text = "ZohoUpdate", TooltipText = "Append Vendors to Zoho ", PrefixIcon = "e-export" }
                };
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }
        private void ConnectToZoho()
        {
            var returnUrl = UriHelper.ToBaseRelativePath(UriHelper.Uri);
            var url = ZohoTokenService.GetAuthUrl(returnUrl);
            UriHelper.NavigateTo(url, true);
        }

        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "ZohoUpdate")
            {
                try
                {
                    SpinnerVisible = true;
                    var token = await ZohoTokenService.GetAccessTokenAsync();
                    var orgId = await ZohoService.GetOrganizationIdAsync(token);

                    SupplierList = await SupplierService.GetSuppliers();
                    foreach (var item in SupplierList)
                    {
                        if (item.SuppZohoVendId == null || item.SuppZohoVendId == "")
                        {

                            var vendorData = new
                            {
                                contact_name = item.SuppName,
                                company_name = item.SuppName,
                                email = item.SuppEMail,
                                phone = item.SuppPhone,
                                website = item.SuppUrl,
                                contact_type = "vendor"

                            };

                            var result = await ZohoService.AppendSupplierToZoho(token, orgId, vendorData);

                            if (result != null)
                            {
                                using var doc = JsonDocument.Parse(result);
                                var contactId = doc.RootElement
                                                   .GetProperty("contact")
                                                   .GetProperty("contact_id")
                                                   .GetString();

                                item.SuppZohoVendId = contactId;
                                await SupplierService.UpdateSupplier(item);
                            }
                        }
                    }
                    SpinnerVisible = false;
                }
                catch (Exception ex)
                {
                    await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                    return;
                }
            }
        }

        private async Task LoadData()
        { 
            SupplierList = await SupplierService.GetSuppliers();
            await SupplierGrid.Refresh();
        }
        public async void DataBoundHandler(BeforeDataBoundArgs<SupplierMaster> args)
        {
            if (!Initial)
            {
                await Task.Delay(100);
                var Idx = await this.SupplierGrid.GetRowIndexByPrimaryKey(Convert.ToDouble(DetailId));
                this.SupplierGrid.SelectRow(Convert.ToInt32(Idx));
            }
            Initial = false;
        }

        public async Task ActionBeginHandler(ActionEventArgs<SupplierMaster> Args)
        {
            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Save))
            {
                this.SpinnerVisible = true;
                if (Args.Action == "Add")
                {
                    DetailId = 0;
                    detailCode = (from bc in SupplierList where bc.SuppCode == Args.Data.SuppCode select bc.SuppCode).FirstOrDefault();
                    if (detailCode == null)
                    {
                        SupplierList = await SupplierService.GetSuppliers();
                        var Qry = (from vBr in SupplierList.OrderByDescending(x => x.SuppCode) select vBr).FirstOrDefault();
                        if (Qry == null)
                        {
                            Args.Data.SuppCode = "0001";
                        }
                        else
                        {
                            if (Convert.ToInt32(Qry.SuppCode) < 9)
                            {
                                Args.Data.SuppCode = "000" + (Convert.ToInt32(Qry.SuppCode) + 1).ToString().Trim();
                            }
                            else
                            {
                                if (Convert.ToInt32(Qry.SuppCode) < 99)
                                {
                                    Args.Data.SuppCode = "00" + (Convert.ToInt32(Qry.SuppCode) + 1).ToString().Trim();
                                }
                                else
                                {
                                    if (Convert.ToInt32(Qry.SuppCode) < 999)
                                    {
                                        Args.Data.SuppCode = "0" + (Convert.ToInt32(Qry.SuppCode) + 1).ToString().Trim();
                                    }
                                    else
                                    {
                                        Args.Data.SuppCode = (Convert.ToInt32(Qry.SuppCode) + 1).ToString().Trim();
                                    }
                                }
                            }
                        }
                        SupplierMaster suppliersaddedit = new SupplierMaster();
                        suppliersaddedit.SuppCode = Args.Data.SuppCode;
                        suppliersaddedit.SuppName = Args.Data.SuppName;
                        suppliersaddedit.SuppPhone= Args.Data.SuppPhone;
                        suppliersaddedit.SuppCrNo = Args.Data.SuppCrNo;
                        suppliersaddedit.SuppContPerson = Args.Data.SuppContPerson;
                        suppliersaddedit.SuppRemarks = Args.Data.SuppRemarks;
                        suppliersaddedit.SuppUrl = Args.Data.SuppUrl;
                        suppliersaddedit.SuppEMail = Args.Data.SuppEMail;
                        suppliersaddedit.SuppAddr = Args.Data.SuppAddr;
                        suppliersaddedit.SuppCountry = Args.Data.SuppCountry;
                        suppliersaddedit.SuppCity = Args.Data.SuppCity;
                        var resp = await SupplierService.AddSupplier(suppliersaddedit);
                        DetailId = resp.SuppId;
                        Args.Data.SuppId = (int)DetailId;
                        await LoadData();
                    }
                    else
                    {
                        WarningContentMessage = "This Supplier Code is already exists! It won't be added again.";
                        Warning.OpenDialog();
                    }
                }
                else
                {
                    if (DetailId > 0)
                    {
                        this.SpinnerVisible = true;
                        var qry = (from bc in SupplierList where bc.SuppCode == Args.Data.SuppCode select bc).FirstOrDefault();
                        if (qry != null)
                        {
                            await SupplierService.UpdateSupplier(suppliersaddedit);
                            await LoadData();

                        }
                        this.SpinnerVisible = false;
                    }
                    //else
                    //{
                    //    WarningContentMessage = "You must select a Supplier";
                    //    Warning.OpenDialog();
                    //}
                }
                //await SupplierGrid.ShowColumnsAsync(ColumnItems);
                this.SpinnerVisible = false;
            }

            if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Delete))
            {
                isDelete = true;
                DelDetailId = Args.Data.SuppId;
                vSuppCode = Args.Data.SuppCode;
                if (vSuppCode !="")
                {
                    ConfirmContentMessage = "Please confirm that you want to Delete  " + Args.Data.SuppName;
                    DialogDelete.OpenDialog();
                }
                else
                {
                    Warning.OpenDialog();
                }
            }
        }
        protected async Task SuppliersSave()
        {
            this.SpinnerVisible = true;
            try
            {
                if (suppliersaddedit.SuppCode == null || suppliersaddedit.SuppCode == "")
                {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select a Supplier";
                    Warning.OpenDialog();
                }
                else
                {
                    await SupplierService.UpdateSupplier(suppliersaddedit);
                }
                await LoadData();

                this.SpinnerVisible = false;
                await this.DialogSuppliersAddEdit.HideAsync();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        public void RowSelectHandler(RowSelectEventArgs<SupplierMaster> args)
        {
            if (isDelete = false)
            {
                DetailId = args.Data.SuppId;
                vSuppCode = args.Data.SuppCode;
            }
            isDelete = true;   
        }

        protected async Task ConfirmDelete(bool DeleteConfirmed)
        {
            this.SpinnerVisible = true;
            await LoadData();
            if (DeleteConfirmed)
            {
                vSuppId = (from qc in SupplierList where qc.SuppCode == vSuppCode select qc.SuppId).FirstOrDefault();
                if (vSuppId > 0)
                {
                    await SupplierService.DeleteSupplier(vSuppId);
                }
            }
            await LoadData();
            this.SpinnerVisible = false;
            //await SupplierGrid.ShowColumnsAsync(ColumnItems);

        }
        public async Task ExcelExport()
        {
            this.SpinnerVisible = true;
            await this.SupplierGrid.ExportToExcelAsync();
            this.SpinnerVisible = false;

        }
        private async Task Navigate(string detCode)
        {
            this.SpinnerVisible = true;
            suppliersaddedit = SupplierList.Where(e => e.SuppCode == detCode ).FirstOrDefault();

            if (suppliersaddedit == null && detCode != "")
            {
                suppliersaddedit = new SupplierMaster();
                suppliersaddedit.SuppCode = detCode;
                var vQry = (from d in SupplierList where d.SuppCode == detCode select d).FirstOrDefault();
                if (vQry != null)
                {
                    suppliersaddedit.SuppName = vQry.SuppName;
                    suppliersaddedit.SuppPhone = vQry.SuppPhone;
                }
                await this.DialogSuppliersAddEdit.ShowAsync();
            }
            else
            {
                await this.DialogSuppliersAddEdit.ShowAsync();
            }
            this.SpinnerVisible = false;
        }
        private async Task CloseDialog()
        {
            await this.DialogSuppliersAddEdit.HideAsync();
        }

        private async Task OnChangeCountry(Syncfusion.Blazor.DropDowns.SelectEventArgs<GenCountry> args)
        {
            this.SpinnerVisible = true;
            cityList = await cityService.GetCities(args.ItemData.CountryCode);
            this.SpinnerVisible = false;
        }
        private void NavigateToPrevious()
        {
            if (vhref != "" && vhref != null)
            {
                UriHelper.NavigateTo(@vhref);
            }
            else
            {
                UriHelper.NavigateTo("index");
            }
        }
    }

}
