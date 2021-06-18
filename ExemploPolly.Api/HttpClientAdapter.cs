using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExemploPolly.Api
{
	public class HttpClientAdapter : IHttpClientAdapter
	{
		private readonly HttpClient _httpClient;

		public HttpClientAdapter()
		{
			_httpClient = new HttpClient
			{
				Timeout = TimeSpan.FromSeconds(30)
			};
		}

		public Task<HttpResponseMessage> Send(HttpRequestMessage httpRequestMessage) => _httpClient.SendAsync(httpRequestMessage);
	}
}