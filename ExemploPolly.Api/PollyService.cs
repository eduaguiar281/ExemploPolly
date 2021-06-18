using System;
using System.Net.Http;
using Polly;
using Polly.Retry;

namespace ExemploPolly.Api
{
	public class PollyService : IPollyService
	{
		private readonly IConfiguracaoService _configuracaoService;

		public PollyService(IConfiguracaoService configuracaoService)
		{
			_configuracaoService = configuracaoService;
		}

		public AsyncRetryPolicy<HttpResponseMessage> TentarTresVezes(int desabilitarErroNaTentativa = 2)
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
					TratarRetentativas(desabilitarErroNaTentativa, retryCount);
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
				.RetryForeverAsync(onRetry: result =>
				{
					LogService.Logar("Tentando novamente...");
				});
		}
	}
}