using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
			var resposta = await policy.ExecuteAsync(() => _httpClientAdapter.Send(ObterHttpRequestMessageRequisicaoApiExemplo()));
			return Retorno(resposta);
		}

		#endregion

		#region Campos

		private readonly IHttpClientAdapter _httpClientAdapter;
		private readonly IPollyService _pollyService;

		#endregion

		#region Construtores
		
		public PollyController(IHttpClientAdapter httpClientAdapter, IPollyService pollyService)
		{
			_httpClientAdapter = httpClientAdapter;
			_pollyService = pollyService;
		}

		#endregion

		#region Métodos

		private HttpRequestMessage ObterHttpRequestMessageRequisicaoApiExemplo()
		{
			var requestMessage = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri("http://localhost:5000/apiexemplo/requisicao")

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