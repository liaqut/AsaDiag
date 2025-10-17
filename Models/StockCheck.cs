using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class StockCheck
{
    public long Id { get; set; }

    public string? RdScanCode { get; set; }

    public string? RdListNo { get; set; }

    public string? RdLotNo { get; set; }

    public DateTime? RdExpiryDate { get; set; }

    public long? RdStkId { get; set; }

    public decimal? RdQty { get; set; }
}
