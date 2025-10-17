using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class TrHead
{
    public long TrhId { get; set; }

    public long? TrhNo { get; set; }

    public string? TrhDispNo { get; set; }

    public DateTime? TrhDate { get; set; }

    public string? TrhUser { get; set; }

    public DateTime? TrhDateAltered { get; set; }

    public bool? TrhApproved { get; set; }

    public string? TrhRemarks { get; set; }
    public bool? TrhExcludeAlertAction { get; set; }
    public string? TrhComp { get; set; }

    public string? TrhBranch { get; set; }
    public virtual ICollection<TrDetail> TrDetails { get; set; } = new List<TrDetail>();
}
