using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class SdelHead
{
    public long SdelId { get; set; }

    /// <summary>
    /// Transaction Number
    /// </summary>
    public long SdelNo { get; set; }

    /// <summary>
    /// Transaction Date
    /// </summary>
    public DateTime? SdelDate { get; set; }

    public string? SdelDispNo { get; set; }

    public string? SdelUser { get; set; }

    public DateTime? SdelDateAltered { get; set; }

    public bool? SdelApproved { get; set; }

    public DateTime? SdelDateFrom { get; set; }

    public DateTime? SdelDateTo { get; set; }
    public string? SdelComp { get; set; }

    public string? SdelBranch { get; set; }

    public virtual ICollection<SdelDetl> SdelDetls { get; set; } = new List<SdelDetl>();
}
