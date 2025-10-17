using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class Customer
{
    public string CustomerNo { get; set; } = null!;

    public string CustomerName { get; set; } = null!;

    public string? CustomerAddress { get; set; }

    public string? CustomerTel { get; set; }

    public string? CustomerFax { get; set; }

    public string? CustomerEmail { get; set; }

    public string? SmCode { get; set; }

    public string? Flag { get; set; }

    public decimal? AccOpAmt { get; set; }

    public string? CustomerNameArb { get; set; }

    public string? CustomerVatNumber { get; set; }

    public decimal? CustomerVatPct { get; set; }

    public string? CustomerAddressArb { get; set; }

    public string? CustomerCrNumber { get; set; }
}
