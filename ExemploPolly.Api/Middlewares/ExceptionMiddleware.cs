using System.Net;
using System.Threading.Tasks;
using ExemploPolly.Api.Services;
using Microsoft.AspNetCore.Http;
using Polly.CircuitBreaker;

namespace ExemploPolly.Api.Middlewares
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;

		public ExceptionMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext httpContext)
		{
			try
			{
				await _next(httpContext);
			}
			catch (CustomApiException ex)
			{
				HandleRequestExceptionAsync(httpContext, ex.StatusCode);
			}
			catch (BrokenCircuitException)
			{
				HandleCircuitBreakerExceptionAsync(httpContext);
			}
		}

		private static void HandleCircuitBreakerExceptionAsync(HttpContext context)
		{
			context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
			context.Response.WriteAsync("Circuit breaker aberto!");
		}

		private static void HandleRequestExceptionAsync(HttpContext context, HttpStatusCode statusCode)
		{
			context.Response.StatusCode = (int)statusCode;
		}
	}
}