using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class GenScanSpec
{
    public long GenId { get; set; }

    public short? GenScanLength { get; set; }

    public short? GenListLength { get; set; }

    public short? GenListStartFrom { get; set; }

    public short? GenLotLength { get; set; }

    public short? GenLotStartFrom { get; set; }

    public short? GenExpiryLength { get; set; }

    public short? GenExpiryStartFrom { get; set; }

    public short? GenExpiryDir { get; set; }

    public short? GenType { get; set; }
}
