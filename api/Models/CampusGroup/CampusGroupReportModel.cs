namespace api.Models.CampusGroup
{
    public class CampusGroupReportModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public double AveragePassed { get; set; }
        public double AverageFailed { get; set; }
    }
}
