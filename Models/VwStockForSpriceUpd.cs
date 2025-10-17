using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class VwStockForSpriceUpd
{
    public string ItemDesc { get; set; } = null!;

    public string ItemClientCode { get; set; } = null!;

    public string? ClientName { get; set; }

    public string ItemListNo { get; set; } = null!;

    public int ItemBatchId { get; set; }

    public decimal? ItemSp { get; set; }

}
