namespace ExemploPolly.Api
{
	public class ConfiguracaoService : IConfiguracaoService
	{
		public bool ErroHabilitado { get; set; }
	}

	public interface IConfiguracaoService
	{
		bool ErroHabilitado { get; set; }
	}
}
