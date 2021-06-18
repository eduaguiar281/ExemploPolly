using System.Data;
using ExemploPolly.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExemploPolly.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ApiExemploController : ControllerBase
	{
		private readonly IConfiguracaoService _configuracaoService;

		public ApiExemploController(IConfiguracaoService configuracaoService)
		{
			_configuracaoService = configuracaoService;
		}

		[HttpGet("Requisicao")]
		public IActionResult Requisicao()
		{
			return _configuracaoService.ErroHabilitado ? BadRequest("BadRequest") : Ok("Ok");
		}

		[HttpGet("RequisicaoPassivelErro")]
		public IActionResult RequisicaoPassivelErro()
		{
			return _configuracaoService.ErroHabilitado ? throw new DataException("Erro na conexão com o banco de dados"): Ok("Ok");
		}

		[HttpGet("habilitarErros")]
		public IActionResult AlterarEstadoErros()
		{
			_configuracaoService.ErroHabilitado = !_configuracaoService.ErroHabilitado;
			return Ok($"Status: {ObterStatusErro()}");
		}

		private string ObterStatusErro() => _configuracaoService.ErroHabilitado ? "erro habilitado" : "erro desabilitado";
	}
}