using System.Net.Http;
using Polly.Retry;

namespace ExemploPolly.Api.Services
{
	public interface IPollyService
	{
		AsyncRetryPolicy<HttpResponseMessage> TentarTresVezes();
		AsyncRetryPolicy<HttpResponseMessage> TentarEternamente();
	}
}