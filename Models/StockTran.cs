using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class StockTran
{
    public long StkId { get; set; }

    public long ItemId { get; set; }

    public string ItemListNo { get; set; } = null!;

    public string ItemLotNo { get; set; } = null!;

    public DateTime ItemExpiryDate { get; set; }

    public string ItemClientCode { get; set; } = null!;

    public string? ItemSuppCode { get; set; }

    public decimal? ItemTrOpQty { get; set; }

    public decimal? ItemTrOpAmt { get; set; }

    public decimal? ItemUp { get; set; }

    public decimal? ItemOpQty { get; set; }

    public decimal? ItemPurQty { get; set; }

    public decimal? ItemPurAmt { get; set; }

    public decimal? ItemDelQty { get; set; }

    public decimal? ItemDelAmt { get; set; }

    public decimal? ItemTrInQty { get; set; }

    public decimal? ItemTrInAmt { get; set; }

    public decimal? ItemTrOutQty { get; set; }

    public decimal? ItemTrOutAmt { get; set; }

    public long? ItemStkIdDesc { get; set; }

    public string? ItemStkIdGrp { get; set; }

    public string? ItemStkIdUnit { get; set; }

    public string? ItemStkIdCat { get; set; }

    public int? ItemBatchId { get; set; }
    public decimal ItemCp { get; set; }
    public int TotBalQty => Convert.ToInt32((ItemTrOpQty ?? 0) + (ItemTrInQty ?? 0) + (ItemPurQty ?? 0) - ((ItemDelQty ?? 0) + (ItemTrOutQty ?? 0)));
    public decimal TotBalAmt => Convert.ToDecimal(((ItemTrOpQty ?? 0) + (ItemTrInQty ?? 0) + (ItemPurQty ?? 0) - ((ItemDelQty ?? 0) + (ItemTrOutQty ?? 0))) * ItemCp);
    public virtual ItemMaster Item { get; set; } = null!;
}
