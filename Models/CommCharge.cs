namespace DigiEquipSys.Models
{
    public partial class CommCharge
    {
        public long CommId { get; set; }

        public DateTime? CommDate { get; set; }

        public string? CommDesc { get; set; }

        public decimal? CommAmt { get; set; }

        public string? CommRemarks { get; set; }
    }
}
