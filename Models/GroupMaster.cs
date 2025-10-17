using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class GroupMaster
{
    public int GrpId { get; set; }

    public string GrpNo { get; set; } = null!;

    public string? GrpDesc { get; set; }

    public string? GrpShortDesc { get; set; }

    public virtual ICollection<CategMaster> CategMasters { get; set; } = new List<CategMaster>();
}
