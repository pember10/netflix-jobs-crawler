using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Uwp.Notifications;

namespace NetflixCrawlerService
{
    public class Worker(ILogger<Worker> logger, DbContextOptions<CrawledDataContext> dbOptions) : BackgroundService
    {
        private readonly ILogger<Worker> _logger = logger;
        private readonly DbContextOptions<CrawledDataContext> _dbOptions = dbOptions;
        private readonly HttpClient _httpClient = new();
        private Timer? _timer;
        private int _minutesBetweenScans = 10;
        private int _secondsToAvoidRateLimiting = 10;

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(_minutesBetweenScans));
            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            Console.WriteLine($"Crawl started at {TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local).AddMinutes(_minutesBetweenScans).ToShortTimeString()}.");

            var sitePositions = CollectJobData();

            var tasks = sitePositions.Select(async sitePosition =>
            {
                using (var context = new CrawledDataContext(dbOptions))
                {
                    await CrawlPositionData(context, sitePosition);
                }
            });

            await Task.WhenAll(tasks);

            using (var context = new CrawledDataContext(dbOptions))
            {
                await SetOldJobsToNoLongerSeen(context, sitePositions);
            }

            Console.WriteLine($"Crawl complete. Waiting for next scan at {TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local).AddMinutes(_minutesBetweenScans).ToShortTimeString()}.");
        }

        private async Task SetOldJobsToNoLongerSeen(CrawledDataContext context, IEnumerable<Position> sitePositions)
        {
            var positionsList = sitePositions.ToList();
            var jobsToMarkAsNoLongerSeen = context.CrawledData
                .AsEnumerable()
                .Where(d => !positionsList.Any(p => p.display_job_id == d.RequisitionId && p.canonicalPositionUrl == d.CanonicalUrl) && d.NoLongerSeen == false);

            foreach (var job in jobsToMarkAsNoLongerSeen)
            {
                job.NoLongerSeen = true;

                Console.WriteLine($"Job doesn't exist anymore: {job.RequisitionId} ({job.JobTitle})");
            }

            await context.SaveChangesAsync();
        }

        private async Task CrawlPositionData(CrawledDataContext context, Position position)
        {
            var existingData = await context.CrawledData
                .FirstOrDefaultAsync(d => d.RequisitionId == position.display_job_id && d.CanonicalUrl == position.canonicalPositionUrl);

            if (existingData == null)
            {
                await CreateNewJob(context, position);
            }
            else
            {
                await UpdateExistingJob(context, existingData);
            }
        }

        private async Task UpdateExistingJob(CrawledDataContext context, CrawledData existingData)
        {
            existingData.LastSeen = DateTime.Now;
            existingData.TimesCrawled++;
            await context.SaveChangesAsync();
        }

        private async Task CreateNewJob(CrawledDataContext context, Position position)
        {

            var newData = new CrawledData
            {
                JobId = (int)position.id,
                CanonicalUrl = position.canonicalPositionUrl,
                Url = $"[REDACTED]",
                JobTitle = position.name,
                Location = position.location,
                RequisitionId = position.display_job_id,
                PostingDate = DateTimeOffset.FromUnixTimeSeconds(position.t_create).DateTime,
                Team = position.department,
                IsRemote = position.work_location_option.Contains("remote"),
                Content = position.job_description,
                FirstSeen = DateTime.Now,
                LastSeen = DateTime.Now,
                TimesCrawled = 1,
                NoLongerSeen = false
            };

            Console.WriteLine($"New job found: {newData.RequisitionId} ({newData.JobTitle})");

            context.CrawledData.Add(newData);
            await context.SaveChangesAsync();

            ShowNotification(title: "New Netflix Job!", jobSummary: $"{newData.JobTitle} ({newData.Team} team) [Req {newData.RequisitionId}]", positionId: newData.JobId);
        }

        private IEnumerable<Position> CollectJobData()
        {
            var result = new List<Position>();

            int startNumber = 0;
            while (true)
            {
                var jobData = GetJobData(startNumber);

                if (jobData == null || !jobData.positions.Any())
                {
                    break;
                }

                result.AddRange(jobData.positions);

                if (jobData.positions.Length < 10)
                {
                    // We've reached the end, so stop crawling
                    break;
                }
                else
                {
                    // Add 10 more to the start number to retrieve the next results
                    startNumber += 10;
                }

                // Pause for a bit to avoid rate limiting
                Thread.Sleep(_secondsToAvoidRateLimiting * 1000);
            }

            return result.AsEnumerable();
        }

        private EightfoldJob? GetJobData(int startNumber)
        {
            Console.WriteLine($"Crawling results {startNumber}-{startNumber + 10}");
            // Specifically looking for software engineering jobs at Netflix, but could potentially be any job
            var url = $"[Redacted]";
            var response = _httpClient.GetStringAsync(url).Result;

            // response is a JSON string, so we need to deserialize it to an EightfoldJob object
            var jobData = System.Text.Json.JsonSerializer.Deserialize<EightfoldJob>(response);
            return jobData;
        }

        public void ShowNotification(string title, string? jobSummary = null, string? url = null, int? positionId = null)
        {
            var notification = new ToastContentBuilder()
                .AddArgument("action", "viewContents")
                .AddArgument("conversationId", 9813)
                .AddText(title);
            //.AddAppLogoOverride(new Uri("ms-appx:///img/Netflix_N.png"), ToastGenericAppLogoCrop.Circle);

            if (jobSummary != null)
            {
                notification.AddText(jobSummary);
            }

            notification.AddText("Jump on it while it's still fresh!");

            if (positionId.HasValue)
            {
                notification.AddButton(new ToastButton()
                     .SetContent("Show Me")
                     .AddArgument("action", "viewPosition")
                     .AddArgument("positionId", positionId.ToString())
                );
            }

            notification.Show();
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }

}