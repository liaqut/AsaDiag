using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class VwPurchaseOrder
{
    public string? PohDispNo { get; set; }

    public DateTime? PohDate { get; set; }

    public string PohCurr { get; set; } = null!;

    public decimal? PohConvRate { get; set; }

    public string SuppName { get; set; } = null!;

    public string? PodListNo { get; set; }

    public string? ClientName { get; set; }
    public string? ClientCode { get; set; }
    public decimal? PodUp { get; set; }

    public decimal? PoQty { get; set; }

    public decimal? PoRcvdQty { get; set; }

    public decimal? PoTotal { get; set; }

    public bool? PohApproved { get; set; }

    public string ItemDesc { get; set; } = null!;

    public string ItemUnit { get; set; } = null!;

    public decimal? PoRcvdTotal { get; set; }
}
