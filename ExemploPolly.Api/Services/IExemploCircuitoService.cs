using System.Threading.Tasks;

namespace ExemploPolly.Api.Services
{
	public interface IExemploCircuitoService
	{
		Task<string> BuscarDado();
	}
}