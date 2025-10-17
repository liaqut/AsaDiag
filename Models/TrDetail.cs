using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class TrDetail
{
    public long TrdId { get; set; }

    public long? TrdTrhId { get; set; }

    public string? TrdListNo { get; set; }

    public string? TrdLotNo { get; set; }

    public DateTime? TrdExpiryDate { get; set; }
    public int? TrdBatchId { get; set; }

    public long? TrdStkIdDesc { get; set; }

    public long? TrdStockStkId { get; set; }

    public string? TrdClientCodeFrom { get; set; }

    public decimal? TrdClientCodeFromUp { get; set; }

    public decimal? TrdClientCodeFromQty { get; set; }

    public string? TrdClientCodeTo { get; set; }

    public decimal? TrdClientCodeToUp { get; set; }

    public bool? TrdAlert { get; set; }

    public long? TrdIdRevJourn { get; set; }

    public bool? TrdAlertStop { get; set; }

    public decimal? TrdBalJournQty { get; set; }
    public decimal TotalPrice => (TrdClientCodeFromQty ?? 0) * (TrdClientCodeFromUp ?? 0);
    public string? TrdLotChange { get; set; }
    public string? TrdReversal { get; set; }
    public bool? TrdRev { get; set; }
    public long? TrdRevSeq { get; set; }
    public bool? TrdHighlightPriceDiff { get; set; }
    public bool? TrdAction { get; set; }

    public virtual TrHead? TrdTrh { get; set; }
}
