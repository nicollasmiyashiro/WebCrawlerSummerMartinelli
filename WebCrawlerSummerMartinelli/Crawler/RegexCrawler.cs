using System.Diagnostics;
using System.Text.RegularExpressions;

namespace WebCrawlerSummerMartinelli.Crawler;

public static partial class RegexCrawler
{
	[GeneratedRegex("<title>(.*?)<\\/title>")]
	private static partial Regex RegexTitle();

	[GeneratedRegex("<a.*?href=\\\"(.*?)\\\".*?>.*?<p.*?>(.*?)<\\/p>.*?<\\/a>")]
	private static partial Regex RegexEvent();

	//TODO: criar commandline pra mudar os eventos, trabalhar com rabbitmq para notificacoes e o rafa recomendou caching
	private static readonly List<string> Events = ["summer", "eletrohits", "FLASH 2000"];

	public static bool IsTitleDifferent(string siteBody)
	{
		var title = RegexTitle().Match(siteBody).Groups[1].Value;
		if (title.Contains("Martinelli"))
			return false;

		Log.Write("Link has changed!", EventLogEntryType.Error);
		return true;
	}

	public static Dictionary<string, string> GetSummerLinks(string siteBody)
	{
		return ValidateSummer(siteBody)
			.ToDictionary(m => m.Groups[1].Value, m => m.Groups[2].Value);
	}

	private static IEnumerable<Match> ValidateSummer(string siteBody)
	{
		return RegexEvent()
			.Matches(siteBody)
			.Where(HasEvent);
	}

	private static bool HasEvent(Match match)
	{
		var anchor = match.Groups[1].Value;
		var paragraph = match.Groups[2].Value;
		
		return Events.Any(e => (HasTextFrom(anchor, e) || HasTextFrom(paragraph, e))
		//&& !ContemTextoDe(paragraph, "ESGOTADO")
		);
	}

	private static bool HasTextFrom(string element, string arrayText)
	{
		return element.Contains(arrayText, StringComparison.OrdinalIgnoreCase);
	}
}
