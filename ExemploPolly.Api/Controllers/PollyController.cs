using System;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using ExemploPolly.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Polly;
using Polly.CircuitBreaker;

namespace ExemploPolly.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PollyController : ControllerBase
	{
		#region Ações
		
		[HttpGet("EfetuarRequisicaoSemPolly")]
		public IActionResult EfetuarRequisicaoSemPolly()
		{
			var requisicao = _httpClientAdapter.Send(ObterHttpRequestMessageRequisicaoApiExemplo()).Result;
			LogService.Logar(ObterMensagemStatusRequisicao(requisicao));
			return Retorno(requisicao);
		}

		[HttpGet("TentarTresVezes")]
		public async Task<IActionResult> TentarTresVezes()
		{
			LogarDivisao();
			var policy = _pollyService.TentarTresVezes();
			var resposta = await policy.ExecuteAsync(() => _httpClientAdapter.Send(ObterHttpRequestMessageRequisicaoApiExemplo()));
			return Retorno(resposta);
		}

		[HttpGet("TentarEternamente")]
		public async Task<IActionResult> TentarEternamente()
		{
			LogarDivisao();
			var policy = _pollyService.TentarEternamente();
			var resposta = await policy.ExecuteAsync(() =>
			{
				return _httpClientAdapter.Send(ObterHttpRequestMessageRequisicaoApiExemplo());
			});
			return Retorno(resposta);
		}

		[HttpGet("CircuitBreaker")]
		public async Task<IActionResult> CircuitBreaker()
		{
			LogarDivisao();

			void OnBreak(Exception exception, TimeSpan timespan)
			{
				LogService.Logar("Circuito aberto");
				_fakeService.SalvarNoBancoDeDadosTemporario();
			}

			void OnReset()
			{
				_fakeService.SalvarNoBancoDeDados();
				LogService.Logar("Circuito restabelecido");
			}

			CircuitBreakerPolicy circuitBreakerPolicy = Policy
				.Handle<Exception>()
				.CircuitBreaker(2, TimeSpan.FromMinutes(1), OnBreak, OnReset);

			circuitBreakerPolicy.Execute((() => _fakeService.SalvarNoBancoDeDados()));
			
			return Ok();
		}



		#endregion

		#region Campos

		private readonly IHttpClientAdapter _httpClientAdapter;
		private readonly IPollyService _pollyService;
		private readonly IFakeService _fakeService;

		#endregion

		#region Construtores
		
		public PollyController(IHttpClientAdapter httpClientAdapter, IPollyService pollyService, IFakeService fakeService)
		{
			_httpClientAdapter = httpClientAdapter;
			_pollyService = pollyService;
			_fakeService = fakeService;
		}

		#endregion

		#region Métodos

		private HttpRequestMessage ObterHttpRequestMessageRequisicaoApiExemplo(string action = "requisicao")
		{
			var requestMessage = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri($"http://localhost:5000/apiexemplo/{action}")

			};

			requestMessage.Headers.Clear();
			return requestMessage;
		}

		private IActionResult Retorno(HttpResponseMessage responseMessage)
		{
			var mensagem = ObterMensagemStatusRequisicao(responseMessage);
			LogService.Logar(mensagem);
			LogarDivisao();
			LogService.Logar(Environment.NewLine);
			return Ok(mensagem);
		}

		private static string ObterMensagemStatusRequisicao(HttpResponseMessage responseMessage)
		{
			return responseMessage.IsSuccessStatusCode ? "Ok" : "Solicitação falhou";
		}

		private void LogarDivisao() => LogService.Logar("===============================================");

		#endregion
	}
}