using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class SystemPage
{
    public int PageId { get; set; }

    public string? PageUrl { get; set; }

    public string? PageDetail { get; set; }

    public string? PageCompType { get; set; }
}
