using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CircuitBreaker.Api.Services
{
	public class ExemploCircuitoService : IExemploCircuitoService
	{
		private readonly HttpClient _httpClient;

		public ExemploCircuitoService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<string> BuscarDado()
		{
			var response = await _httpClient.SendAsync(ObterHttpRequestMessageRequisicaoApiExemplo());
			if (response.StatusCode != HttpStatusCode.OK) throw new CustomApiException(response.StatusCode);
			return await response.Content.ReadAsStringAsync();
		}

		private HttpRequestMessage ObterHttpRequestMessageRequisicaoApiExemplo()
		{
			var requestMessage = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri($"http://localhost:55555/apiexemplo/RequisicaoPassivelErro")

			};

			requestMessage.Headers.Clear();
			return requestMessage;
		}
	}
}