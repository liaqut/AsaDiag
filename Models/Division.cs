using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class Division
{
    public int LocId { get; set; }

    public string? LocCode { get; set; }

    public string? LocBranchCode { get; set; }

    public string? LocDesc { get; set; }

    public string? LocAddress { get; set; }

    public string? LocCity { get; set; }

    public string? LocState { get; set; }

    public virtual Branch? LocBranchCodeNavigation { get; set; }
}
