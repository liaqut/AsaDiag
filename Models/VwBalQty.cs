using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class VwBalQty
{
    public string ItemListNo { get; set; } = null!;

    public string ItemLotNo { get; set; } = null!;

    public DateTime ItemExpiryDate { get; set; }

    public string ItemClientCode { get; set; } = null!;

    public decimal? BalQty { get; set; }
}
