using System;
using System.Collections.Generic;

namespace DigiEquipSys.Models;

public partial class PoDetail
{
    public long PodId { get; set; }

    public long PodPohId { get; set; }

    public string? PodListNo { get; set; }


    public decimal? PodUp { get; set; }

    public decimal? PodQty { get; set; }

    public decimal? PodDiscPct { get; set; }

    public decimal? PodDiscAmt { get; set; }

    public decimal? PodUpAftDisc { get; set; }

    public decimal? PodRcvdQty { get; set; }

    public decimal? PodRtndQty { get; set; }

    public decimal? PodInvdQty { get; set; }

    public decimal? PodGstPct { get; set; }

    public long? PodStkIdDesc { get; set; }

    public string? PodStkIdGrp { get; set; }

    public string? PodStkIdCat { get; set; }

    public string? PodStkIdUnit { get; set; }

    public decimal? PodAmount { get; set; }
    public string? PodHsnCode { get; set; }
    public decimal? PodGstAmt { get; set; }
    public virtual PoHead PodPoh { get; set; } = null!;
}
