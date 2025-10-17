using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class VwTransfer
{
    public string? TrhDispNo { get; set; }

    public DateTime? TrhDate { get; set; }

    public bool? TrhApproved { get; set; }

    public string? TrdListNo { get; set; }

    public string? TrdLotNo { get; set; }

    public DateTime? TrdExpiryDate { get; set; }
    public int? TrdBatchId { get; set; }
    public string ItemDesc { get; set; } = null!;

    public string ItemUnit { get; set; } = null!;

    public string? GrpDesc { get; set; }

    public string? CatDesc { get; set; }

    public string? ClientFrom { get; set; }

    public decimal? TrdClientCodeFromUp { get; set; }

    public decimal? QtyFrom { get; set; }

    public decimal? AmtFrom { get; set; }

    public string? ClientTo { get; set; }

    public decimal? TrdClientCodeToUp { get; set; }

    public bool? TrdAlert { get; set; }

    public decimal? DiffPurchPrice { get; set; }

    public decimal? NonAlertAmt { get; set; }

    public decimal? AlertAmt { get; set; }

    public decimal? DiffAmt { get; set; }

    public long? TrdStkIdDesc { get; set; }

    public long? TrdStockStkId { get; set; }

    public string? TrdClientCodeFrom { get; set; }

    public string? TrdClientCodeTo { get; set; }

    public long TrdId { get; set; }

    public long? TrdIdRevJourn { get; set; }

    public bool? TrdAlertStop { get; set; }

    public decimal? TrdBalJournQty { get; set; }

    public decimal? TrdClientCodeFromQty { get; set; }

    public decimal? OrderQty { get; set; }

    public string? TrdLotChange { get; set; }
    public string? TrdReversal { get; set; }
    public bool? TrdRev { get; set; }
    public decimal? RevAmt { get; set; }
    public bool? TrdHighlightPriceDiff { get; set; }
    public string? ItemGrpCode { get; set; }
    public long? TrdRevSeq { get; set; }
    public bool? TrdAction { get; set; }

}
