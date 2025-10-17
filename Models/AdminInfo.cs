using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class AdminInfo
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? LastLogin { get; set; }

    public string? CreatedOn { get; set; }

    public string? UpdatedOn { get; set; }

    public string? RoleId { get; set; }

    public string? LocId { get; set; }

    public virtual Branch? Branch { get; set; }

    public virtual ICollection<SysPagesControl> SysPagesControls { get; set; } = new List<SysPagesControl>();
}
