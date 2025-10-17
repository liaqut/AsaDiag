using DigiEquipSys.Interfaces;
using DigiEquipSys.Models;
using DigiEquipSys.Services;
using DigiEquipSys.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Charts.Chart.Internal;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
//using BoldReports.Processing.Objec.Models;

namespace DigiEquipSys.Pages
{
    public partial class Customer_pg
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

        private SfGrid<ClientMaster>? CustomerGrid;
        private SfGrid<ClientCity>? ClientCityGrid;
        private long vDetailId;
        private long vDelDetailId;
        private string detailCode = "";
        public string? smName { get; set; }

        private bool SpinnerVisible { get; set; }=false;
        public bool IsEdit { get; set; } = true;

        [Inject]
        public IGenCountryService? countryService { get; set; }
        public IEnumerable<GenCountry>? countryList;

        [Inject]
        public IGenCityService? cityService { get; set; }
        public IEnumerable<GenCity>? cityList;

        [Parameter] public string vhref { get; set; }
        public string MyRacc { get; set; } = "01";
        //public string[] ColumnItems = new string[] { "Code" };

        SfDialog? DialogClientsAddEdit;
        [Inject] public IClientService? ClientService { get; set; }
        public IEnumerable<ClientMaster>? AdetailList;
        public ClientMaster clientsaddedit = new();
        private int vDelClientId = 0;
        private string vDelClientCode = "";

        public string[] CustType = { "Retail", "Dealer" };
        private string? myLoc;
        public ClientCity clientcityaddedit = new();
        public List<ClientCity>? clientcitylist;
        public bool Initial { get; set; } = true;
        bool isDelete = false;
        private List<Object> ToolbarItems { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                myLoc = await sessionStorage.GetItemAsync<string>("adminLoc");
                this.SpinnerVisible = true;
                AdetailList = await ClientService.GetClients();
                countryList = await countryService.GetGenCountryDetails();
                cityList = await cityService.GetGenCityDetails();
                this.SpinnerVisible = false;
                ToolbarItems = new List<object>()
                {
                    "Add", "Delete", "Edit", "Cancel", "Update","Print",
                    new ItemModel() { Text = "ZohoUpdate", TooltipText = "Append Customers to Zoho ", PrefixIcon = "e-export" }
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

                    AdetailList = await ClientService.GetClients();
                    foreach (var item in AdetailList)
                    {
                        if (item.ClientZohoClientId == null || item.ClientZohoClientId == "")
                        {

                            var clientData = new
                            {
                                contact_name = item.ClientName,
                                company_name = item.ClientName,
                                email = item.ClientEmail,
                                phone = item.ClientTel,
                                website = item.ClientUrl,
                                contact_type = "customer"

                            };

                            var result = await ZohoService.AppendCustomerToZoho(token, orgId, clientData);



                            if (result != null)
                            {
                                using var doc = JsonDocument.Parse(result);
                                var contactId = doc.RootElement
                                                   .GetProperty("contact")
                                                   .GetProperty("contact_id")
                                                   .GetString();

                                item.ClientZohoClientId = contactId;
                                await ClientService.UpdateClient(item);
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


        public async void DataBoundHandler(BeforeDataBoundArgs<ClientMaster> args)
        {
            try
            {
                if (!Initial)
                {
                    await Task.Delay(100);
                    var Idx = await this.CustomerGrid.GetRowIndexByPrimaryKey(Convert.ToDouble(vDetailId));
                    this.CustomerGrid.SelectRow(Convert.ToInt32(Idx));
                }
                Initial = false;
            }
			catch (Exception ex)
			{
				await JSRuntime.InvokeVoidAsync("alert", ex.Message);
				return;
			}

		}
		public async Task ActionBeginHandler(ActionEventArgs<ClientMaster> Args)
        {
            try
            {
                //if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Add) || Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.BeginEdit))
                //{
                //    await CustomerGrid.HideColumnsAsync(ColumnItems);
                //}
                //if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Cancel))
                //{
                //    await CustomerGrid.ShowColumnsAsync(ColumnItems);
                //}
                if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Save))
                {
                    this.SpinnerVisible = true;
                    if (Args.Action == "Add")
                    {
                        vDetailId = 0;
                        //detailCode = (from bc in AdetailList where bc.ClientId == Args.Data.ClientId select bc.ClientCode).FirstOrDefault();
                        if (Args.Data.ClientId == 0)
                        {

                            var Qry = (from vBr in AdetailList.OrderByDescending(x => x.ClientCode) select vBr).FirstOrDefault();
                            if (Qry == null)
                            {
                                Args.Data.ClientCode = "0001";
                            }
                            else
                            {
                                if (Convert.ToInt32(Qry.ClientCode) < 9)
                                {
                                    Args.Data.ClientCode = "000" + (Convert.ToInt32(Qry.ClientCode) + 1).ToString().Trim();
                                }
                                else
                                {
                                    if (Convert.ToInt32(Qry.ClientCode) < 99)
                                    {
                                        Args.Data.ClientCode = "00" + (Convert.ToInt32(Qry.ClientCode) + 1).ToString().Trim();
                                    }
                                    else
                                    {
                                        if (Convert.ToInt32(Qry.ClientCode) < 999)
                                        {
                                            Args.Data.ClientCode = "0" + (Convert.ToInt32(Qry.ClientCode) + 1).ToString().Trim();
                                        }
                                        else
                                        {
                                            Args.Data.ClientCode = (Convert.ToInt32(Qry.ClientCode) + 1).ToString().Trim();
                                        }
                                    }
                                }
                            }
                            ClientMaster clientsaddedit = new ClientMaster();
                            //clientsaddedit.ClientId = 0;
                            clientsaddedit.ClientCode = Args.Data.ClientCode;
                            clientsaddedit.ClientName = Args.Data.ClientName;
                            clientsaddedit.ClientTel = Args.Data.ClientTel;
                            clientsaddedit.ClientVendCode = Args.Data.ClientVendCode;
                            clientsaddedit.ClientContactPerson = Args.Data.ClientContactPerson;
                            clientsaddedit.ClientAddr = Args.Data.ClientAddr;
                            clientsaddedit.ClientEmail = Args.Data.ClientEmail;
                            clientsaddedit.ClientUrl = Args.Data.ClientUrl;
                            clientsaddedit.ClientCrNumber = Args.Data.ClientCrNumber;
                            clientsaddedit.ClientRemarks = Args.Data.ClientRemarks;
                            await ClientService.AddClient(clientsaddedit);
                            vDetailId = clientsaddedit.ClientId;
                            StateHasChanged();
                            await CustomerGrid.Refresh();
                        }
                        else
                        {
                            WarningContentMessage = "This Customer Code is already exists! It won't be added again.";
                            Warning.OpenDialog();
                        }
                    }
                    else
                    {
                        if (vDetailId > 0)
                        {
                            var qry = (from bc in AdetailList where bc.ClientId == vDetailId select bc).FirstOrDefault();
                            if (qry != null)
                            {
                                if (qry.ClientId == vDetailId)
                                {
                                    await ClientService.UpdateClient(Args.Data);
                                }
                                else
                                {
                                    WarningContentMessage = "This Customer Code is already exists! You can not overridden.";
                                    Warning.OpenDialog();
                                }
                            }
                        }
                        //else
                        //{
                        //    WarningContentMessage = "You must select a Customer";
                        //    Warning.OpenDialog();
                        //}
                    }
                    //await CustomerGrid.ShowColumnsAsync(ColumnItems);
                    this.SpinnerVisible = false;
                    StateHasChanged();
                }

                if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Delete))
                {
                    isDelete = true;
                    vDelDetailId = Args.Data.ClientId;
                    vDelClientCode = Args.Data.ClientCode;

                    if (vDelClientCode != "")
                    {

                        ConfirmContentMessage = "Please confirm that you want to Delete  " + Args.Data.ClientName;
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
		public void RowSelectHandler(RowSelectEventArgs<ClientMaster> args)
        {
            if (isDelete = false)
            {

                vDetailId = args.Data.ClientId;
                detailCode = args.Data.ClientCode;
            }
            isDelete = true;
        }

        protected async Task ConfirmDelete(bool DeleteConfirmed)
        {
            try
            {
                this.SpinnerVisible = true;
                AdetailList = await ClientService.GetClients();
                if (DeleteConfirmed)
                {
                    vDelClientId = (from qc in AdetailList where qc.ClientCode == vDelClientCode select qc.ClientId).FirstOrDefault();
                    if (vDelClientId > 0)
                    {
                        await ClientService.DeleteClient(vDelClientId);
                    }
                }
                AdetailList = await ClientService.GetClients();
                this.SpinnerVisible = false;
                //await CustomerGrid.ShowColumnsAsync(ColumnItems);
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
                await this.CustomerGrid.ExportToExcelAsync();
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
            if (vhref != "" && vhref != null)
            {
                UriHelper.NavigateTo(@vhref);
            }
            else
            {
                UriHelper.NavigateTo("index");
            }
        }
        private async Task Navigate(int detCode)
        {
            try
            {
                this.SpinnerVisible = true;
                clientsaddedit = AdetailList.Where(e => e.ClientId == detCode).FirstOrDefault();
                if (clientsaddedit == null)
                {
                    clientsaddedit = new ClientMaster();
                    clientcityaddedit = new ClientCity();
                    await this.DialogClientsAddEdit.ShowAsync();
                }
                else
                {
                    int vClId = clientsaddedit.ClientId;
                    clientcitylist = await myclientcityservice.GetClientCities(vClId);
                    clientcityaddedit.ClientId = vClId;
                    await this.DialogClientsAddEdit.ShowAsync();
                }
                this.SpinnerVisible = false;
                IsEdit = true;
            }
            catch (Exception ex)
            {
				await JSRuntime.InvokeVoidAsync("alert", ex.Message);
				return;
			}
		}
        protected async Task ClientsSave()
        {
            await ClientCityGrid.EndEditAsync();
            this.SpinnerVisible = true;
            try
            {

                if (clientsaddedit.ClientCode == null || clientsaddedit.ClientCode == "")
                {
                    WarningHeaderMessage = "Warning!";
                    WarningContentMessage = "Please select a Customer";
                    Warning.OpenDialog();
                }
                else
                {
                    var res1 = await myclientcityservice.UpdateClientCity(clientcityaddedit);
                    var res= await ClientService.UpdateClient(clientsaddedit);
                    if (res=="ERROR")
                    {
                        WarningHeaderMessage = "Warning!";
                        WarningContentMessage = "Duplicate Entry; You may have duplicated the city/place for this client..  ";
                        Warning.OpenDialog();
                    }
                }
                AdetailList = await ClientService.GetClients();
                this.SpinnerVisible = false;
                await this.DialogClientsAddEdit.HideAsync();
                IsEdit = true;
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        public async Task ActionBeginHandlerClientCity(ActionEventArgs<ClientCity> Args)
        {
            try
            {
                if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Save))
                {
                    this.SpinnerVisible = true;
                    if (Args.Action == "Add")
                    {
                        var q1 = (from xc in clientcitylist where xc.ClientCityId == Args.Data.ClientCityId select xc).FirstOrDefault();
                        if (q1 == null)
                        {
                            ClientCity clientcityaddedit = new ClientCity();
                            clientcityaddedit.CityId = Args.Data.CityId;
                            clientcityaddedit.ClientId = clientsaddedit.ClientId;
                            await myclientcityservice.AddClientCity(clientcityaddedit);
                            StateHasChanged();
                        }
                        else
                        {
                            if (q1.ClientCityId == 0)
                            {
                                ClientCity clientcityaddedit = new ClientCity();
                                clientcityaddedit.CityId = Args.Data.CityId;
                                clientcityaddedit.ClientId = clientsaddedit.ClientId;
                                await myclientcityservice.AddClientCity(clientcityaddedit);
                                StateHasChanged();
                            }
                            else
                            {
                                WarningContentMessage = "This City is already exists! It won't be added again.";
                                Warning.OpenDialog();
                            }
                        }
                    }
                    else
                    {
                        if (vDetailId > 0)
                        {
                            await myclientcityservice.UpdateClientCity(Args.Data);
                            StateHasChanged();

                        }
                    }
                    this.SpinnerVisible = false;
                }
                if (Args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Delete))
                {
                    this.SpinnerVisible = true;
                    int vDel = Convert.ToInt32(Args.Data.ClientCityId);
                    await myclientcityservice.DeleteClientCity(vDel);
                    clientcitylist = await myclientcityservice.GetClientCities(Convert.ToInt32(Args.Data.ClientId));
                    this.SpinnerVisible = false;
                }
            }
			catch (Exception ex)
			{
				await JSRuntime.InvokeVoidAsync("alert", ex.Message);
				return;
			}
		}
		private async Task CloseDialog()
        {
            await this.DialogClientsAddEdit.HideAsync();
            IsEdit = true;
        }
        private async Task OnChangeCountry(Syncfusion.Blazor.DropDowns.SelectEventArgs<GenCountry> args)
        {
            try
            {
                cityList = await cityService.GetCities(args.ItemData.CountryCode);
            }
            catch (Exception ex)
            {
				await JSRuntime.InvokeVoidAsync("alert", ex.Message);
				return;
			}
		}

    }
}
