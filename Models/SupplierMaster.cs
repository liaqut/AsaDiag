using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class SupplierMaster
{
    public int SuppId { get; set; }

    public string SuppCode { get; set; } = null!;

    public string SuppName { get; set; } = null!;

    public string? SuppContPerson { get; set; }

    public string? SuppCity { get; set; }

    public string? SuppCountry { get; set; }

    public string? SuppAddr { get; set; }

    public string? SuppPhone { get; set; }

    public string? SuppEMail { get; set; }

    public string? SuppUrl { get; set; }

    public string? SuppRemarks { get; set; }

    public string? SuppCrNo { get; set; }
    public string? SuppZohoVendId { get; set; }

    public virtual GenCity? GenCity { get; set; }
}
