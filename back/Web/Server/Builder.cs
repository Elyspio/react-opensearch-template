using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Newtonsoft.Json.Converters;
using OpenSearch.Api.Abstractions.Helpers;
using OpenSearch.Api.Abstractions.Interfaces.Injections;
using OpenSearch.Api.Adapters.Injections;
using OpenSearch.Api.Core.Injections;
using OpenSearch.Api.Db.Injections;
using OpenSearch.Api.Search.Injections;
using OpenSearch.Api.Web.Filters;
using OpenSearch.Api.Web.Processors;
using OpenSearch.Api.Web.Utils;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Net;
using System.Text.Json.Serialization;

namespace OpenSearch.Api.Web.Server;

public class ServerBuilder
{
	private readonly string frontPath = Env.Get("FRONT_PATH", "/front");

	public ServerBuilder(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		builder.WebHost.ConfigureKestrel((_, options) =>
			{
				options.Listen(IPAddress.Any, 4000, listenOptions =>
					{
						// Use HTTP/3
						listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
					}
				);
			}
		);


		// Setup CORS
		builder.Services.AddCors(options =>
			{
				options.AddPolicy("Cors", b =>
					{
						b.AllowAnyOrigin();
						b.AllowAnyHeader();
						b.AllowAnyMethod();
					}
				);

				options.DefaultPolicyName = "Cors";
			}
		);

		builder.Services.AddModule<SearchModule>(builder.Configuration);
		builder.Services.AddModule<AdapterModule>(builder.Configuration);
		builder.Services.AddModule<CoreModule>(builder.Configuration);
		builder.Services.AddModule<DatabaseModule>(builder.Configuration);


		// Setup Logging
		builder.Host.UseSerilog((_, lc) => lc
			.MinimumLevel.Debug()
			.Filter.ByExcluding(e => e.Level == LogEventLevel.Debug && e.Properties["SourceContext"].ToString().Contains("Microsoft"))
			.Enrich.FromLogContext()
			.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level} {SourceContext:l}] {Message:lj}{NewLine}{Exception}", theme: AnsiConsoleTheme.Sixteen)
		);

		// Convert Enum to String 
		builder.Services.AddControllers(o =>
				{
					o.Conventions.Add(new ControllerDocumentationConvention());
					o.OutputFormatters.RemoveType<StringOutputFormatter>();
					o.Filters.Add<HttpExceptionFilter>();
				}
			)
			.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
			.AddNewtonsoftJson(x => x.SerializerSettings.Converters.Add(new StringEnumConverter()));

		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddOpenApiDocument(document =>
		{
			document.DocumentName = "Example.Api";
			document.Title = "Example.Api";

			document.SchemaProcessors.Add(new NullableSchemaProcessor());
			document.OperationProcessors.Add(new NullableOperationProcessor());
		});

		builder.Services.AddSignalR(options => { options.EnableDetailedErrors = true; })
			.AddJsonProtocol(options =>
				{
					options.PayloadSerializerOptions.IncludeFields = true;
					options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
				}
			);

		// Setup SPA Serving
		if (builder.Environment.IsProduction()) Console.WriteLine($"Server in production, serving SPA from {frontPath} folder");

		Application = builder.Build();
	}

	public WebApplication Application { get; }
}