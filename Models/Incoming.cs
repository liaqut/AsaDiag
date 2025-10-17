using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class Incoming
{
    public string? Value { get; set; }

    public double? Qty { get; set; }

    public DateTime? DateEntered { get; set; }

    public string? ListNumber { get; set; }

    public string? KitName { get; set; }

    public string? LotNumber { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string? F8 { get; set; }
}
