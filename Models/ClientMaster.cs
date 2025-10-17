using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class ClientMaster
{
    public int ClientId { get; set; }

    public string ClientCode { get; set; } = null!;

    public string? ClientName { get; set; }

    public string? ClientVendCode { get; set; }

    public string? ClientContactPerson { get; set; }

    public string? ClientAddr { get; set; }

    public string? ClientTel { get; set; }

    public string? ClientEmail { get; set; }

    public string? ClientUrl { get; set; }

    public string? ClientCrNumber { get; set; }

    public string? ClientRemarks { get; set; }
    public string? ClientZohoClientId { get; set; }

}
