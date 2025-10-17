using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class PoHead
{
    public long PohId { get; set; }

    public long PohNo { get; set; }

    public string? PohDispNo { get; set; }

    public DateTime? PohDate { get; set; }

    public string? PohRemarks { get; set; }
    public int? PohCustId { get; set; }
    public int? PohVendId { get; set; }

    public string? PohVendRef { get; set; }

    public bool? PohApproved { get; set; }

    public string? PohUser { get; set; }

    public DateTime? PohDateAltered { get; set; }

    public string PohCurr { get; set; } = null!;

    public decimal? PohConvRate { get; set; }

    public string? PohComp { get; set; }

    public string? PohBranch { get; set; }

    public virtual ICollection<PoDetail> PoDetails { get; set; } = new List<PoDetail>();

    public virtual GenCurrency PohCurrNavigation { get; set; } = null!;
}
