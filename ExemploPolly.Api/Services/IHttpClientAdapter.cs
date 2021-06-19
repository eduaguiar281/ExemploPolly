using System.Net.Http;
using System.Threading.Tasks;

namespace ExemploPolly.Api.Services
{
	public interface IHttpClientAdapter
	{
		Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage);
	}
}