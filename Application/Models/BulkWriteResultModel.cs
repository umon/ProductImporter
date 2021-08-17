namespace Application.Models
{
    public class BulkWriteResultModel
    {
        public long Inserted { get; set; }
        public long Updated { get; set; }
        public long Deleted { get; set; }
    }
}
