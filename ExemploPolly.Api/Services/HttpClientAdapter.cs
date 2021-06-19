using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExemploPolly.Api.Services
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

		public Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage) => _httpClient.SendAsync(httpRequestMessage);
	}
}