using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class SdelDetl
{
    public long SdelDetId { get; set; }

    public long? SdelHeadId { get; set; }

    public DateTime? SdelDate { get; set; }

    public string? SdelListNo { get; set; }

    public string? SdelLotNo { get; set; }

    public DateTime? SdelExpiryDate { get; set; }

    public string? SdelClientCode { get; set; }

    public long? SdelStkIdDesc { get; set; }

    public string? SdelClientVendCode { get; set; }

    public string? SdelProdCode { get; set; }

    public string? SdelListNoProd { get; set; }

    public decimal? SdelQty { get; set; }

    public decimal? SdelUprice { get; set; }

    public decimal? TotalCost => Convert.ToDecimal((SdelQty ?? 0) * (SdelUprice ?? 0));
    public virtual SdelHead? SdelHead { get; set; }
}
