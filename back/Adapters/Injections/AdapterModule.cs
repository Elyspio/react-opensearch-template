using OpenSearch.Api.Abstractions.Interfaces.Injections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenSearch.Api.Adapters.AuthenticationApi;
using OpenSearch.Api.Adapters.Configs;

namespace OpenSearch.Api.Adapters.Injections;

public class AdapterModule : IDotnetModule
{
	public void Load(IServiceCollection services, IConfiguration configuration)
	{
		var conf = new EndpointConfig();
		configuration.GetSection(EndpointConfig.Section).Bind(conf);

		services.AddHttpClient<IJwtClient, JwtClient>(client => { client.BaseAddress = new Uri(conf.Authentication); });
	}
}