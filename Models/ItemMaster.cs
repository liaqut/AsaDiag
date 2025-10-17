using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class ItemMaster
{
    public long ItemId { get; set; }

    public string? ItemGrpCode { get; set; }

    public string? ItemCatCode { get; set; }

    public string? ItemScanCode { get; set; }

    public string ItemListNo { get; set; } = null!;

    public string ItemClientCode { get; set; } = null!;

    public string ItemDesc { get; set; } = null!;

    public string ItemUnit { get; set; } = null!;

    public string? ItemProdCode { get; set; }

    public int? ScanCodeLength { get; set; }

    public string? ItemSuppCode { get; set; }

    public decimal? ItemCostPrice { get; set; }

    public decimal? ItemSellPrice { get; set; }

    public string? ItemListNoProd { get; set; }

    public decimal? ItemCostPricePrev { get; set; }

    public decimal? ItemSellPricePrev { get; set; }
    public decimal? ItemGst { get; set; }
    public string? ItemHsnCode { get; set; }
    public string? ItemZohoItemId { get; set; }

    public virtual ICollection<StockTran> StockTrans { get; set; } = new List<StockTran>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
