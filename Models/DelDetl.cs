using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class DelDetl
{
    public long DelDetId { get; set; }

    public long? DelHeadId { get; set; }

    public string? DelScanCode { get; set; }

    public string? DelListNo { get; set; }

    public string? DelLotNo { get; set; }

    public DateTime? DelExpiryDate { get; set; }

    public string? DelClientCode { get; set; }

    public int? DelBatchId { get; set; }

    public decimal? DelQty { get; set; }

    public decimal? DelUprice { get; set; }

    public decimal? DelPurchPrice { get; set; }

    public long? DelStkIdDesc { get; set; }

    public string? DelStkIdGrp { get; set; }

    public string? DelStkIdCat { get; set; }

    public string? DelStkIdUnit { get; set; }

    public long? DelStkId { get; set; }

    public long? DelStockStkId { get; set; }
    public decimal TotalPrice => (DelQty ?? 0) * (DelUprice ?? 0);
    public virtual DelHead? DelHead { get; set; }
}
