using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class CurrentInventoryLevel
{
    public string? Value { get; set; }

    public decimal? IncomingStock { get; set; }

    public decimal? OutgoingStock { get; set; }

    public double? OnHandStock { get; set; }

    public string? ListNumber { get; set; }

    public string? KitName { get; set; }

    public string? LotNumber { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string? Expired { get; set; }

    public string? ProductCode { get; set; }
}
