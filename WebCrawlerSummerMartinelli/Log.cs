using System.Diagnostics;

namespace WebCrawlerSummerMartinelli;

public static class Log
{
	private static string SourceProjectName { get; set; } = "TemSummerNoMartinelli";

	public static void CreateLog()
	{
		if (!EventLog.SourceExists(SourceProjectName))
			EventLog.CreateEventSource(SourceProjectName, "application");
	}

	public static void Write(string message, EventLogEntryType type = EventLogEntryType.Information)
	{
		EventLog.WriteEntry(
			source: SourceProjectName,
			message: message,
			type: type
		);
	}
}