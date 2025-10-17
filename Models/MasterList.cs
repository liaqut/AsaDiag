using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class MasterList
{
    public string? ListNumber { get; set; }

    public string? ClientCode { get; set; }

    public string? CustomerName { get; set; }

    public string? VendorName { get; set; }

    public string? SuppCode { get; set; }

    public string? VendorGroup { get; set; }

    public string? GrpCode { get; set; }

    public string? ProductDescription { get; set; }

    public string? Unit { get; set; }

    public double? PurchasePrice { get; set; }

    public double? SellingPrice { get; set; }

    public string? Category { get; set; }

    public string? CatCode { get; set; }
}
