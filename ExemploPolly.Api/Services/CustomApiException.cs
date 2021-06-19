using System;
using System.Net;

namespace ExemploPolly.Api.Services
{
	public class CustomApiException : Exception
	{
		public HttpStatusCode StatusCode { get; }

		public CustomApiException(HttpStatusCode statusCode)
		{
			StatusCode = statusCode;
		}
	}
}