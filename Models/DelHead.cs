using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class DelHead
{
    public long DelId { get; set; }

    public long DelNo { get; set; }

    public DateTime? DelDate { get; set; }

    public int? DelClientId { get; set; }

    public int? DelCityId { get; set; }

    public string? PoNumber { get; set; }

    public DateTime? PoDate { get; set; }

    public string? DelDispNo { get; set; }

    public string? DelUser { get; set; }

    public DateTime? DelDateAltered { get; set; }

    public bool? DelApproved { get; set; }
    public string? DelComp { get; set; }

    public string? DelBranch { get; set; }
    public virtual ICollection<DelDetl> DelDetls { get; set; } = new List<DelDetl>();
}
