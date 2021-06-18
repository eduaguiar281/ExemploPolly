using System;
using System.Data;
using System.Net.Http;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace ExemploPolly.Api.Services
{
	public class PollyService : IPollyService
	{
		private readonly IConfiguracaoService _configuracaoService;

		public PollyService(IConfiguracaoService configuracaoService)
		{
			_configuracaoService = configuracaoService;
		}

		public AsyncRetryPolicy<HttpResponseMessage> TentarTresVezes()
		{
			_configuracaoService.ErroHabilitado = true;

			return Policy
				.HandleResult<HttpResponseMessage>(message => !message.IsSuccessStatusCode)
				.WaitAndRetryAsync(new[]
				{
					TimeSpan.FromSeconds(1),
					TimeSpan.FromSeconds(2),
					TimeSpan.FromSeconds(4)
				}, (outcome, timeSpan, retryCount, context) =>
				{
					TratarRetentativas(2, retryCount);
				});
		}

		private void TratarRetentativas(int desabilitarErroNaTentativa, int retryCount)
		{
			if (retryCount == desabilitarErroNaTentativa) _configuracaoService.ErroHabilitado = false;
			LogService.Logar($"Tentativa número {retryCount} - Erro {ObterMensagemErroHabilitado()}");
		}

		private string ObterMensagemErroHabilitado() => _configuracaoService.ErroHabilitado ? "habilitado" : "desabilitado";

		public AsyncRetryPolicy<HttpResponseMessage> TentarEternamente()
		{
			_configuracaoService.ErroHabilitado = true;

			return Policy
				.HandleResult<HttpResponseMessage>(message => !message.IsSuccessStatusCode)
				.WaitAndRetryForeverAsync(
					retryAttempt => TimeSpan.FromSeconds(3),
					(exception, timespan, context) =>
					{
						LogService.Logar("Erro. Tentando novamente...");
					});
		}

		public AsyncCircuitBreakerPolicy CircuitBreaker(Action onBreak, Action onReset)
		{
			return Policy
				.Handle<DataException>()
				.CircuitBreakerAsync(1, TimeSpan.FromMinutes(1), (exception, span) => onBreak(), onReset);
		}
	}
}