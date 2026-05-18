# WebCrawler Summer Martinelli

A **Windows Background Service** built with .NET that automatically monitors [Sympla](https://www.sympla.com.br) and the Martinelli event agenda for **Summer Eletrohits** ticket availability — and notifies you instantly via **WhatsApp** and **Windows Toast** when tickets go on sale.

***

## Problem Context

Getting tickets for Summer Eletrohits at Martinelli is competitive — they sell out fast. Checking the event pages manually is tedious and unreliable. This service solves that by running quietly in the background on Windows, polling both the **Sympla public API** and **Martinelli's Linktree agenda** every 10 minutes. As soon as tickets become available, you receive a WhatsApp message (via CallMeBot) and a Windows desktop notification with the direct link.

***

## How It Works

The service follows a two-track monitoring strategy:

1. **Linktree Crawler** — Fetches the `linktr.ee/Agenda_e_Ingressos` page and uses Regex to extract any Sympla links related to Summer Eletrohits events.
2. **Sympla API** — Queries the Sympla public API (`/public/v3/events`) directly for events named "SUMMER ELETROHITS" and retrieves their ticket URLs.
3. **Ticket Verification** — For each discovered Sympla event link, calls the internal Sympla BFF endpoint (`/api/event-bff/purchase/event/{id}/tickets`) and parses the JSON to check if `availableQty > 0`.
4. **Notification** — If tickets are found, sends a WhatsApp message via [CallMeBot](https://www.callmebot.com/) and displays a Windows Toast notification with a button to copy the event link.

The polling cycle repeats every **10 minutes** via a `BackgroundService` worker. All events are logged to the **Windows Event Log** under the source `TemSummerNoMartinelli`.

***

## Technologies

| Technology | Purpose |
|---|---|
| **.NET 8 / C#** | Application framework |
| **Microsoft.Extensions.Hosting** | Windows Background Service (`IHostedService`) |
| **System.Text.RegularExpressions** | Source-generated Regex for HTML parsing |
| **System.Text.Json** | JSON deserialization of Sympla ticket data |
| **Polly** | HTTP resilience: retry (exponential backoff) + fallback policies |
| **Microsoft.Toolkit.Uwp.Notifications** | Windows Toast notifications |
| **CallMeBot API** | Free WhatsApp notification delivery |
| **Sympla Public API v3** | Event discovery by name |
| **Windows Event Log** | Persistent service logging |

***

## Configuration

Before running, fill in the required credentials in the source files:

### 1. WhatsApp via CallMeBot (`Notification.cs`)

```csharp
var phoneNumber = "YOUR_PHONE_NUMBER"; // e.g., 5511999999999
var apikey = "YOUR_CALLMEBOT_APIKEY";
```

To get your CallMeBot API key, send `I allow callmebot to send me messages` to **+34 644 44 84 18** on WhatsApp, then wait for the API key reply.

### 2. Sympla API Key (`SymplaInterface.cs`)

```csharp
private static readonly string _apiKey = "YOUR_SYMPLA_TOKEN";
```

Generate a token at [sympla.com.br/dev](https://www.sympla.com.br/dev).

***

## How to Run

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8) or later
- Windows OS (required for Windows Toast notifications and Event Log)

### Run Locally (Development)

```bash
# Clone the repository
git clone https://github.com/your-username/WebCrawlerSummerMartinelli.git
cd WebCrawlerSummerMartinelli

# Restore dependencies
dotnet restore

# Run the application
dotnet run
```

The service will start polling immediately and log output to the Windows Event Log. You can view logs in **Event Viewer → Windows Logs → Application**, filtering by source `TemSummerNoMartinelli`.

### Install as a Windows Service (Production)

```bash
# Publish a self-contained executable
dotnet publish -c Release -r win-x64 --self-contained

# Install as a Windows Service (run as Administrator)
sc create TemSummerNoMartinelli binPath="C:\path\to\publish\WebCrawlerSummerMartinelli.exe"

# Start the service
sc start TemSummerNoMartinelli
```

To stop and remove the service:

```bash
sc stop TemSummerNoMartinelli
sc delete TemSummerNoMartinelli
```

***

## Disclaimer

This project is intended for personal use only. Respect Sympla's and Linktree's Terms of Service when using automated crawling. The polling interval is set to 10 minutes to avoid excessive load on external services.
