using System;
using System.Net;
using System.Net.Http;
using ApiCore;
using ExemploPolly.Api.Middlewares;
using ExemploPolly.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Retry;

namespace ExemploPolly.Api
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
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "ExemploPolly.Api", Version = "v1" });
			});

			// Circuit breaker
			services.AddHttpClient<IExemploCircuitoService, ExemploCircuitoService>()
				.AddPolicyHandler(EsperarTentar())
				.AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(3, TimeSpan.FromSeconds(10)));

			services.AddSingleton<IConfiguracaoService, ConfiguracaoService>();
			services.AddSingleton<IHttpClientAdapter, HttpClientAdapter>();
			services.AddScoped<IPollyService, PollyService>();
		}

		public static AsyncRetryPolicy<HttpResponseMessage> EsperarTentar()
		{
			return Policy
				.HandleResult<HttpResponseMessage>(message => message.StatusCode == HttpStatusCode.InternalServerError)
				.WaitAndRetryForeverAsync((i, context) => TimeSpan.FromSeconds(2), (outcome, timespan, retryCount, context) =>
				{
					Console.ForegroundColor = ConsoleColor.Blue;
					Console.WriteLine($"Tentando pela {retryCount} vez!");
					Console.ForegroundColor = ConsoleColor.White;
				});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExemploPolly.Api v1"));
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