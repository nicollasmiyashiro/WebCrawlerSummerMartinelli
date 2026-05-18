using Microsoft.Toolkit.Uwp.Notifications;
using WebCrawlerSummerMartinelli.Http;

namespace WebCrawlerSummerMartinelli;

public class Notification(HttpCall httpCall)
{
	private readonly HttpCall _httpCall = httpCall;

	public void SendMessageWhatsapp(string message, string link = "")
	{
		message = message.Replace(' ', '+');
		link = link != "" ? $"+Segue+o+link:+{link}" : "";
		var phoneNumber = "";
		var apikey = "";
		_ = _httpCall.Get($"https://api.callmebot.com/whatsapp.php?phone={phoneNumber}&text={message}{link}&apikey={apikey}");
	}

	public void ShowToast(string message, string link = "")
	{
		new ToastContentBuilder()
			.AddArgument("action", "openApp")
			.AddText("Summer no Martinelli")
			.AddText(message)
			.AddButton(new ToastButton()
				.SetContent("Pegar o link")
				.AddArgument("action", "copy")
				.AddArgument("message", link))
			.Show();
	}
}