using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class RcptDetail
{
    public long RdId { get; set; }

    public long? RdRhId { get; set; }

    public string? RdScanCode { get; set; }

    public string? RdListNo { get; set; }

    public string? RdLotNo { get; set; }

    public DateTime? RdExpiryDate { get; set; }

    public decimal? RdUp { get; set; }

    public decimal? RdQty { get; set; }

    public long? RdStkIdDesc { get; set; }

    public string? RdStkIdGrp { get; set; }

    public string? RdStkIdCat { get; set; }

    public string? RdStkIdUnit { get; set; }

    public long? RdStkId { get; set; }

    public long? RdStockStkId { get; set; }

    public string? RdSuppCode { get; set; }
    public string? RdPohDispNo { get; set; }
    public string? RdClientCode { get; set; }

    public string? RdVendInvNo { get; set; }

    public DateTime? RdVendInvDate { get; set; }
    public decimal TotalPrice => (RdQty ?? 0) * (RdUp ?? 0);
    public virtual RcptHead? RdRh { get; set; }
    public List<PoHead> PohList { get; set; } = new();
}
