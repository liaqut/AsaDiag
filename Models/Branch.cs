using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class Branch
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } = null!;

    public string? BranchDesc { get; set; }

    public string? BranchDescArb { get; set; }

    public string? BranchAddress { get; set; }

    public string? BranchCity { get; set; }

    public string? BranchState { get; set; }

    public string? BranchCountry { get; set; }

    public string? BranchPoBox { get; set; }

    public string? BranchPhone { get; set; }

    public string? BranchCrNo { get; set; }

    public string? BranchVatNo { get; set; }

    public string? BranchEmail { get; set; }

    public string? BranchUrl { get; set; }

    public int? BranchAdminId { get; set; }

    public string? SetupPassword { get; set; }

    public virtual AdminInfo BranchNavigation { get; set; } = null!;
    public virtual ICollection<Division> Divisions { get; set; } = new List<Division>();

}
