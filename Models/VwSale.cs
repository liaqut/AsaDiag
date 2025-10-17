using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class VwSale
{
    public string? DelDispNo { get; set; }

    public DateTime? DelDate { get; set; }

    public string ItemDesc { get; set; } = null!;

    public string ItemUnit { get; set; } = null!;

    public string? ClientName { get; set; }

    public string? DelListNo { get; set; }

    public string? DelLotNo { get; set; }

    public DateTime? DelExpiryDate { get; set; }
    public int DelBatchId { get; set; }

    public decimal? DelQty { get; set; }

    public decimal? DelUprice { get; set; }

    public string? CityName { get; set; }

    public decimal? DelTotal { get; set; }

    public int ClientId { get; set; }

    public string? ClientVendCode { get; set; }

    public string? ItemProdCode { get; set; }

    public long ItemId { get; set; }

    public string? DelClientCode { get; set; }

    public bool? DelApproved { get; set; }

    public string? ItemGrpCode { get; set; }

    public string? ItemCatCode { get; set; }

    public string? CatDesc { get; set; }

    public string? GrpDesc { get; set; }

    public decimal? DelPurchPrice { get; set; }

    public decimal? PurchTotal { get; set; }

    public decimal? Gross { get; set; }

    public decimal? Margin { get; set; }
    public string? DelStkIdGrp { get; set; }
    public long? DelStockStkId { get; set; }


}
