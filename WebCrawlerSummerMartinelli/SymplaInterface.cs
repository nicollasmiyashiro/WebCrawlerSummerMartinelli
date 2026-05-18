using System.Text.Json;
using WebCrawlerSummerMartinelli.Http;

namespace WebCrawlerSummerMartinelli;

public class SymplaInterface(HttpCall httpCall)
{
	private static readonly string _apiKey = "";
	private readonly HttpCall _httpCall = httpCall;
	private readonly string _evento = "SUMMER ELETROHITS";

	private async Task<string> GetEvents()
	{
		return await _httpCall.Get("https://api.sympla.com.br/public/v3/events?name=" + Uri.EscapeDataString(_evento)
			, new() { { "s_token", _apiKey } });
	}

	public IEnumerable<string> ListEvents()
	{
		dynamic? json = JsonSerializer.Deserialize<object>(GetEvents().Result);

		Console.WriteLine(json);
		if (!json?["data"]?.HasValues)
			yield break;

		Console.WriteLine(json?["data"]);
		foreach (var evento in json["data"])
		{
			if (!evento["name"].ToString().Contains(_evento, StringComparison.OrdinalIgnoreCase))
				continue;

			yield return evento["url"];
		}
	}
}