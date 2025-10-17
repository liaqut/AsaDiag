using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class GenCity
{
    public int CityId { get; set; }

    public string CityCode { get; set; } = null!;

    public string? CityName { get; set; }

    public string CityCountryCode { get; set; } = null!;

    public virtual GenCountry CityCountryCodeNavigation { get; set; } = null!;

    public virtual ICollection<SupplierMaster> SupplierMasters { get; set; } = new List<SupplierMaster>();
}
