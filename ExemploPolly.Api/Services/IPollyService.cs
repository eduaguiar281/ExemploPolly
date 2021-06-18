using System;
using System.Net.Http;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace ExemploPolly.Api.Services
{
	public interface IPollyService
	{
		AsyncRetryPolicy<HttpResponseMessage> TentarTresVezes();
		AsyncRetryPolicy<HttpResponseMessage> TentarEternamente();

		AsyncCircuitBreakerPolicy CircuitBreaker(Action onBreak, Action onReset);
	}
}