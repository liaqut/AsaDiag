using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class StockForJournal
{
    public string ItemDesc { get; set; } = null!;

    public string? ClientName { get; set; }

    public string ItemListNo { get; set; } = null!;

    public string ItemLotNo { get; set; } = null!;

    public DateTime ItemExpiryDate { get; set; }

    public decimal? BalQty { get; set; }

    public long StkId { get; set; }

    public int ItemBatchId { get; set; }
}
