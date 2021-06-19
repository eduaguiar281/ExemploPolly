using System;
using System.Net;
using System.Net.Http;
using ApiCore;
using CircuitBreaker.Api.Middlewares;
using CircuitBreaker.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Retry;

namespace CircuitBreaker.Api
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{

			services.AddControllers();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "Circuit breaker.Api", Version = "v1" });
			});

			services.AddSingleton<IConfiguracaoService, ConfiguracaoService>();
			services.AddSingleton<IHttpClientAdapter, HttpClientAdapter>();


			///////////////////////////////////////////////////////////////////////////
			// Circuit breaker
			services.AddHttpClient<IExemploCircuitoService, ExemploCircuitoService>()
				.AddPolicyHandler(EsperarTentar())
				.AddTransientHttpErrorPolicy(p =>
				{
					var numeroDeFalhasParaAbrirCircuito = 3;
					var segundosAberto = TimeSpan.FromSeconds(10);
					return p.CircuitBreakerAsync(numeroDeFalhasParaAbrirCircuito, segundosAberto, OnBreak, OnReset);
				});
			///////////////////////////////////////////////////////////////////////////
			///////////////////////////////////////////////////////////////////////////
		}

		private void OnReset(Context obj)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Reset no circuito");
			Console.ForegroundColor = ConsoleColor.White;
		}

		private void OnBreak(DelegateResult<HttpResponseMessage> arg1, TimeSpan arg2, Context arg3)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Circuito aberto!");
			Console.ForegroundColor = ConsoleColor.White;
		}

		public static AsyncRetryPolicy<HttpResponseMessage> EsperarTentar()
		{
			return Policy
				.HandleResult<HttpResponseMessage>(message => message.StatusCode == HttpStatusCode.InternalServerError)
				.WaitAndRetryForeverAsync((i, context) => TimeSpan.FromSeconds(2), (outcome, timespan, retryCount, context) =>
				{
					Console.ForegroundColor = ConsoleColor.Blue;
					Console.WriteLine("Vou fazer outra tentativa");
					Console.ForegroundColor = ConsoleColor.White;
				});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CircuitBreaker.Api v1"));
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseMiddleware<ExceptionMiddleware>();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}