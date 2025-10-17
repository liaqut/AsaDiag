using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class ClientCity
{
    public int ClientCityId { get; set; }

    public int? ClientId { get; set; }

    public int? CityId { get; set; }
}
