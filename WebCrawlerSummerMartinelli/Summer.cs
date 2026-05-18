using System.Diagnostics;
using WebCrawlerSummerMartinelli.Crawler;
using WebCrawlerSummerMartinelli.Http;

namespace WebCrawlerSummerMartinelli;

public class Summer(HttpCall httpClient, Notification notification, SymplaInterface symplaHttp)
{
	private readonly HttpCall _httpCall = httpClient;
	private readonly Notification _notification = notification;
	private readonly SymplaInterface _symplaHttp = symplaHttp;

	public void VerifyIfHasEvent()
	{
		VerifyIfHasSummerOnSympla();
		CheckSummerSympla();
	}

	private async Task<Dictionary<string, string>> GetLinksFromMartinelli()
	{
		var html = await _httpCall.Get("https://linktr.ee/Agenda_e_Ingressos");

		if (IsHtmlEmpty(html) || RegexCrawler.IsTitleDifferent(html))
			return [];

		return RegexCrawler.GetSummerLinks(html);
	}

	private void CheckSummerSympla()
	{
		foreach (var link in _symplaHttp.ListEvents())
			_notification.SendMessageWhatsapp($"There are tickets!", link);
	}

	private static bool IsHtmlEmpty(string html)
	{
		if (html != String.Empty)
			return false;

		Log.Write("HTML is empty.", EventLogEntryType.Error);
		return true;
	}

	private async void VerifyIfHasSummerOnSympla()
	{
		var linksSummer = await GetLinksFromMartinelli();

		foreach (var item in linksSummer)
			VerifySymplaTickets(item.Key);

		if (linksSummer.Count != 0)
			Log.Write($"\nLinks summer: {string.Join("\n", linksSummer)}");
		else
			Log.Write("We are aware from summer event.");
	}

	private async void VerifySymplaTickets(string link)
	{
		var idLink = link[link.LastIndexOf('/')..];

		var html = await _httpCall.Get($"https://event-page.svc.sympla.com.br/api/event-bff/purchase/event{idLink}/tickets");
		var hasTicket = JsonCrawler.HasAnyTicket(html);

		Log.Write($"Tickets are{(hasTicket ? "" : "n't")} available at {link}\n");

		if (!hasTicket)
			return;

		_notification.SendMessageWhatsapp($"There are tickets!", link);
	}
}