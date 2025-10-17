using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class VwReceipt
{
    public string? RhDispNo { get; set; }

    public DateTime? RhDate { get; set; }

    public bool? RhApproved { get; set; }

    public string? RdListNo { get; set; }

    public string? RdLotNo { get; set; }

    public DateTime? RdExpiryDate { get; set; }

    public string? ItemDesc { get; set; }

    public string? ItemUnit { get; set; }

    public string? GrpDesc { get; set; }

    public string? CatDesc { get; set; }

    public string? SuppName { get; set; }

    public string? ClientName { get; set; }

    public string? RdVendInvNo { get; set; }

    public DateTime? RdVendInvDate { get; set; }

    public decimal? RdQty { get; set; }

    public decimal? RdUp { get; set; }

    public decimal? RdTotal { get; set; }

    public string? ItemGrpCode { get; set; }

    public string? RdClientCode { get; set; }

    public long? RdStkId { get; set; }
    public string? RdStkIdGrp { get; set; }
    public long? RdStockStkId { get; set; }

    }
