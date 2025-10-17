using DigiEquipSys.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
namespace DigiEquipSys.Pages
{
    public partial class ChkListInv_pg
    {
        [CascadingParameter]
        public EventCallback notify { get; set; }
        private string? myUser;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    myUser = await sessionStorage.GetItemAsync<string>("adminEmail");
                    await notify.InvokeAsync();
                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                    return;
                }
            }
        }
        private ItemMaster? im;

        [Parameter]
        public string? vEdit { get; set; }
        [Parameter] public long RcptvouId { get; set; }
        public class RcptDetailSumm
        {
            public string? RdScanCode { get; set; }
            public string? RdListNo { get; set; }
            public string? RdLotNo { get; set; }
            public DateTime RdExpiryDate { get; set; }
            public long RdStkId { get; set; }
            public decimal RdQty { get; set; }
            public long RdId { get; set; }
        }

        private SfGrid<RcptDetailSumm>? RcptDetGridSumm;
        protected List<ItemMaster> ItemMasterList = new();
        protected List<StockCheck> rcptvoudetails = new();
        protected List<RcptDetailSumm> rcptvoudetailsSumm = new();
        private RcptDetailSumm rcptaddedit = new();
        private RcptHead? rcpt = new();
        private RcptDetail rcptDet = new();
        //private SupplierMaster supp = new();
        private Stock stk = new();
        //private string? suppname { get; set; }
        private string? vRdListNo { get; set; }
        private string? vRdLotNo { get; set; }
        private DateTime vRdExpiryDate { get; set; }
        private bool isApprove = true;
        private bool isDisApprove = true;
        private string Custom { get; set; }
        private bool showEditButton = false;
        private RcptDetailSumm selectedDetail;
        public bool IsVisRole { get; set; } = true;
        private string? myRole;
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {

                myRole = await sessionStorage.GetItemAsync<string>("adminRo");
                ItemMasterList = await myItemMaster.GetItemMasters();

                rcptvoudetails = await myStockCheckService.GetStockChecks();
                var tt = rcptvoudetails
                      .GroupBy(r => new { r.RdScanCode, r.RdListNo, r.RdLotNo, r.RdExpiryDate, r.RdStkId })
                      .Select(g => new RcptDetailSumm
                      {
                          RdScanCode = g.Key.RdScanCode,
                          RdListNo = g.Key.RdListNo,
                          RdLotNo = g.Key.RdLotNo,
                          RdExpiryDate = Convert.ToDateTime(g.Key.RdExpiryDate),
                          RdStkId = Convert.ToInt64(g.Key.RdStkId),
                          RdQty = g.Sum(r => Convert.ToDecimal(r.RdQty)),
                          RdId = g.Min(r => Convert.ToInt64(r.Id)),
                      }).ToList();
                rcptvoudetailsSumm = tt;
                TotalItems = rcptvoudetailsSumm.Count();
                RcptDetGridSumm?.Refresh();
                await InvokeAsync(StateHasChanged);
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
        public async Task Previous()
        {
            try
            {
                NavigationManager.NavigateTo($"stkchk_pg");
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                return;
            }
        }

        private void OnRowSelected(RowSelectEventArgs<RcptDetailSumm> args)
        {
            selectedDetail = args.Data;
        }
        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Text == "Excel Export") //Id is combination of Grid's ID and itemname
            {
                try
                {
                    //this.SpinnerVisible = true;
                    if (RcptDetGridSumm != null)
                    {
                        await RcptDetGridSumm.ExportToExcelAsync();
                    }
                    //this.SpinnerVisible = false;
                }
                catch (Exception ex)
                {
                    await JSRuntime.InvokeVoidAsync("alert", ex.Message);
                    return;
                }
            }
        }
    }
}
