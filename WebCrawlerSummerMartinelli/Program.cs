using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebCrawlerSummerMartinelli.Http;

namespace WebCrawlerSummerMartinelli;

public class Program
{
	public static async Task Main(string[] args)
	{
		Log.CreateLog();

		var builder = Host.CreateApplicationBuilder(args);
		builder.Services.AddWindowsService(opt =>
		{
			opt.ServiceName = "TemSummerNoMartinelli";
		});

		builder.Services.AddHostedService<Worker>();
		builder.Services.AddSingleton<HttpCall>();
		builder.Services.AddSingleton<Notification>();
		builder.Services.AddSingleton<SymplaInterface>();
		builder.Services.AddSingleton<Summer>();

		var host = builder.Build();
		await host.RunAsync();
	}
}