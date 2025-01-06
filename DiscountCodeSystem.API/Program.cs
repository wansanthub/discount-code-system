using DiscountCodeSystem.API;
using DiscountCodeSystem.Infrastructure.Service;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = CreateHostBuilder(args);
        var app = builder.Build();

        app.Run();
    }
    public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices(services =>
                    {
                        services.AddSingleton<DiscountCodeService>();
                        services.AddMemoryCache();
                        services.AddSignalR();
                    });

                    webBuilder.Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapHub<DiscountHub>("/discountHub");
                        });
                    });
                });
    
}