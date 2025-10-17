using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class RcptHead
{
    public long RhId { get; set; }

    public long? RhNo { get; set; }

    public string? RhDispNo { get; set; }

    public DateTime? RhDate { get; set; }

    public string? RhRemarks { get; set; }

    public string? RhUser { get; set; }

    public DateTime? RhDateAltered { get; set; }

    public bool? RhApproved { get; set; }

    public string? RhPoNo { get; set; }

    public int? RhSuppId { get; set; }

    public string? RhVendDelNote { get; set; }

    public string? RhVendInvNote { get; set; }
    public string? RhComp { get; set; }

    public string? RhBranch { get; set; }
    public virtual ICollection<RcptDetail> RcptDetails { get; set; } = new List<RcptDetail>();
}
