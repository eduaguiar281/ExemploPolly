using System.Net.Http;
using System.Threading.Tasks;

namespace ExemploPolly.Api
{
	public interface IHttpClientAdapter
	{
		Task<HttpResponseMessage> Send(HttpRequestMessage httpRequestMessage);
	}
}