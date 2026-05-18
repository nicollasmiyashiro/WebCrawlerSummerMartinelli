using Polly;
using Polly.Fallback;
using Polly.Retry;
using System.Net;

namespace WebCrawlerSummerMartinelli.Http;

public class HttpCall
{
	private readonly HttpClient _httpClient;

	public HttpCall()
	{
		_httpClient = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
		_httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "deflate, gzip");
	}

	public async Task<string> Get(string url, Dictionary<string, string>? headers = null)
	{
		AddHeaders(headers);
		var wrap = Policy.WrapAsync(GetRetryPolicyMessage(), GetRetryPolicyHttp());
		var resposta = await wrap.ExecuteAsync(async () =>
			await _httpClient.GetAsync(url)
		);
		return await resposta.Content.ReadAsStringAsync();
	}

	private void AddHeaders(Dictionary<string, string>? headers = null)
	{
		if (headers == null)
			return;

		foreach (var item in headers)
			_httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
	}

	private AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicyHttp()
	{
		return Policy
			.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
			.Or<HttpRequestException>()
			.WaitAndRetryAsync(3, retryAttempt =>
				TimeSpan.FromSeconds(Math.Pow(3, retryAttempt)));
	}

	private AsyncFallbackPolicy<HttpResponseMessage> GetRetryPolicyMessage()
	{
		return Policy<HttpResponseMessage>
			.Handle<HttpRequestException>()
			.FallbackAsync(fallbackValue: new HttpResponseMessage()
			{
				StatusCode = HttpStatusCode.NotFound,
				Content = new StringContent(string.Empty)
			},
			onFallbackAsync: (outcome, context) =>
			{
				Console.WriteLine("\nException Caught!");
				Console.WriteLine("Message :{0} ", outcome.Exception?.Message);
				return Task.CompletedTask;
			});
	}
}