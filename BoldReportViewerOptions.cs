namespace DigiEquipSys
{
    public class BoldReportViewerOptions
    {
        public string ReportName { get; set; }
        public string ServiceURL { get; set; }
        public List<JSReportParameter>? Parameters { get; set; }
    }
    public class JSReportParameter
    {
        public string? Name { get; set; }
        public List<string>? Values { get; set; }
    }
}