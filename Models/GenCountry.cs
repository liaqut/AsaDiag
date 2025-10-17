using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class GenCountry
{
    public int CountryId { get; set; }

    public string CountryCode { get; set; } = null!;

    public string? CountryName { get; set; }

    public virtual ICollection<GenCity> GenCities { get; set; } = new List<GenCity>();
}
