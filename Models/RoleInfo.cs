﻿using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class RoleInfo
{
    public int RoleId { get; set; }

    public string RoleCode { get; set; } = null!;

    public string RoleName { get; set; } = null!;
}
