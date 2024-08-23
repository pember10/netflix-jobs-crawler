using Microsoft.EntityFrameworkCore;

namespace NetflixCrawlerService
{
    public class CrawledDataContext : DbContext
    {
        public CrawledDataContext() : base() { }

        public CrawledDataContext(DbContextOptions<CrawledDataContext> options) : base(options) { }

        public CrawledDataContext(DbSet<CrawledData> crawledData)
        {
            CrawledData = crawledData;
        }

        public DbSet<CrawledData> CrawledData { get; set; }
    }

    public class CrawledData
    {
        public int Id { get; set; }
        public long? JobId { get; set; }
        public string? CanonicalUrl { get; set; }
        public string? JsonUrl { get; set; }
        public string? JobTitle { get; set; }
        public string? Location { get; set; }
        public string? RequisitionId { get; set; }
        public DateTime? PostingDate { get; set; }
        public string? Team { get; set; }
        public bool? IsRemote { get; set; }
        public string? Compensation { get; set; }
        public string? Description { get; set; }
        public DateTime FirstSeen { get; set; }
        public DateTime LastSeen { get; set; }
        public int? TimesCrawled { get; set; }
        public bool? NoLongerSeen { get; set; }
    }
}
