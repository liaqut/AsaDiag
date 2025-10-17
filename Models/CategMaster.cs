using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class CategMaster
{
    public int CatId { get; set; }

    public string? CatGrpNo { get; set; }

    public string? CatNo { get; set; }

    public string? CatDesc { get; set; }

    public string? CatShortDesc { get; set; }

    public virtual GroupMaster? CatGrpNoNavigation { get; set; }
}
