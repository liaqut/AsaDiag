using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class SysPagesControl
{
    public int SysPagesId { get; set; }

    public int? SysPagesControlId { get; set; }

    public string SysPagesEmail { get; set; } = null!;

    public bool? SysPagesAuthorized { get; set; }

    public virtual AdminInfo SysPagesEmailNavigation { get; set; } = null!;
}
