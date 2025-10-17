using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class MenuInfo
{
    public int MenuId { get; set; }

    public int? ParentMenuId { get; set; }

    public string? PageName { get; set; }

    public string? MenuName { get; set; }

    public string? IconName { get; set; }
}
