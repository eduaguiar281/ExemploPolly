using System.Data;

namespace ExemploPolly.Api.Services
{
	public class FakeService : IFakeService
	{
		private readonly IConfiguracaoService _configuracaoService;

		public FakeService(IConfiguracaoService configuracaoService)
		{
			_configuracaoService = configuracaoService;
		}

		public void SalvarNoBancoDeDados()
		{
			if (_configuracaoService.ErroHabilitado) throw new DataException("Erro de conexão com o banco de dados");
			LogService.Logar("Salvou no banco de dados");
		}

		public void VoltarASalvarNoBancoDeDados()
		{
			if (_configuracaoService.ErroHabilitado) throw new DataException("Erro de conexão com o banco de dados");
			LogService.Logar("Voltou a salvar no banco de dados");
		}

		public void SalvarNoBancoDeDadosTemporario()
		{
			LogService.Logar("Salvou no banco de dados temporário");
		}
	}
}