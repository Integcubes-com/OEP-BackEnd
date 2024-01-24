namespace ActionTrakingSystem.DTOs
{
    public class TilDetailReportDto
    {
        public string siteName { get; set; }
        public int? siteId { get; set; }
        public string unit { get; set; }
        public int? actions { get; set; }
        public int? closed { get; set; }
        public int? opened { get; set; }
        public int? inProgress { get; set; }
    }
}
