using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenSearch.Api.Abstractions.Interfaces.Injections;

namespace OpenSearch.Api.Search.Injections;

public class SearchModule : IDotnetModule
{
	public void Load(IServiceCollection services, IConfiguration configuration)
	{
		var nsp = typeof(SearchModule).Namespace!;
		var baseNamespace = nsp[..nsp.LastIndexOf(".")];
		services.Scan(scan => scan
			.FromAssemblyOf<SearchModule>()
			.AddClasses(classes => classes.InNamespaces(baseNamespace + ".Searches"))
			.AsImplementedInterfaces()
			.WithSingletonLifetime()
		);
	}
}