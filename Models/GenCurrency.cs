using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class GenCurrency
{
    public short CurrId { get; set; }

    public string CurrShortName { get; set; } = null!;

    public string? CurrLongName { get; set; }

    public virtual ICollection<PoHead> PoHeads { get; set; } = new List<PoHead>();
}
