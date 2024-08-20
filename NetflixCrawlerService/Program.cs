using NetflixCrawlerService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddDbContext<CrawledDataContext>(options =>
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("CrawledDataConnection")), ServiceLifetime.Singleton);
    })
    .Build()
    .Run();
