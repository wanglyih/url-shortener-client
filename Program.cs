
using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace URLShortener_client
{

    internal sealed class Program
    {
        private static IConfiguration Configuration { get; set; }

        private static async Task Main(string[] args)
        {
            Configuration = new ConfigurationBuilder()
              .SetBasePath(AppContext.BaseDirectory)
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .Build();

            var host = CreateHostBuilder(args).Build();
            host.StartAsync();

            var iterations = Int32.Parse(Configuration["Iterations"]);
            var maxConcurrency = 200; 
            var semaphore = new SemaphoreSlim(maxConcurrency);

            var tasks = new List<Task>();

            for (int i = 0; i < iterations; i++)
            {
                await semaphore.WaitAsync();
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await ProcessRequest(host);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }

            await Task.WhenAll(tasks);

            await host.WaitForShutdownAsync();
        }

        private static async Task ProcessRequest(IHost host)
        {
            var baseSampleUrl = Configuration["SampleUrl"];
            var sequentialGuid = SequentialGuid.Instance.Get();
            var sequentialUrl = baseSampleUrl + sequentialGuid;
            var clientArg = HttpUtility.UrlEncode(sequentialUrl);

            var apiClient = host.Services.GetRequiredService<IApiClient>();
            var result = await apiClient.GetResponse(clientArg);
            Console.WriteLine(result.Response.ShortUrl);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
             .ConfigureLogging(logging =>
             {
                 logging.ClearProviders();
                 logging.AddConsole();
                 logging.AddConfiguration(Configuration.GetSection("Logging"));
             })
            .ConfigureServices((hostContext, services) =>
            {
                

                services.AddHttpClient<IApiClient, ApiClient>("ApiClient", client =>
                {
                    client.BaseAddress = new Uri(Configuration["RestApi:BaseUrl"]); 
                    client.DefaultRequestHeaders.Add("Accept", "application/json");                    

                    // optional settings
                    client.Timeout = TimeSpan.FromSeconds(30);
                    client.DefaultRequestVersion = new Version(1, 0);
                    client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact;
                    client.MaxResponseContentBufferSize = 1_000_000;
                });


                services.AddScoped<IApiClient, ApiClient>();

            });
        }
}




