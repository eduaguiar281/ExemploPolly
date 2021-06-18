namespace ExemploPolly.Api.Services
{
	public interface IFakeService
	{
		void SalvarNoBancoDeDados();

		void VoltarASalvarNoBancoDeDados();

		void SalvarNoBancoDeDadosTemporario();
	}
}