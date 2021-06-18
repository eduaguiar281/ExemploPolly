using System.Net.Http;
using Polly.Retry;

namespace ExemploPolly.Api
{
	public interface IPollyService
	{
		AsyncRetryPolicy<HttpResponseMessage> TentarTresVezes(int desabilitarErroNaTentativa = 2);
		AsyncRetryPolicy<HttpResponseMessage> TentarEternamente();
	}
}