using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebCrawlerSummerMartinelli.Crawler;

public class JsonCrawler
{
	public static bool HasAnyTicket(string siteBody)
	{
		var json = JsonSerializer.Deserialize<IEnumerable<SymplaTicket>>(siteBody);

		if (json == null)
		{
			Console.WriteLine("Json is null!");
			return false;
		}

		return json.Any(ticket =>
		{
			return ticket.AvailableQty > 0;
		});
	}

	private class SymplaTicket
	{
		[JsonPropertyName("availableQty")]
		public int AvailableQty { get; set; }
	}
}
