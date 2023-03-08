using Microsoft.Extensions.Configuration;
using OpenSearch.Api.Search.Configs;
using OpenSearch.Client;
using OpenSearch.Net;

namespace OpenSearch.Api.Search.Searches.Internal;

public class BaseSearchEngine
{
	public BaseSearchEngine(IConfiguration configuration)
	{
		var config = configuration.GetRequiredSection(SearchConfig.Section).Get<SearchConfig>()!;

		var connectionPool = new SingleNodeConnectionPool(new(config.Uri));
		var settings = new ConnectionSettings(connectionPool).EnableDebugMode();

		_client = new(settings);
	}

	protected OpenSearchClient _client { get; }


	async protected Task CreateIndexIfMissing<T>(string name) where T : class
	{
		if (!_client.Indices.Exists(name).Exists)
		{
			var r = await _client.Indices.CreateAsync(name, c => c.Map(m => m.AutoMap<T>()));
		}
	}
}