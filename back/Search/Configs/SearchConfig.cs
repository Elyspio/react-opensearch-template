namespace OpenSearch.Api.Search.Configs;

public sealed class SearchConfig
{
	public const string Section = "Search";
	public required string Username { get; set; }
	public required string Password { get; set; }
	public required string Uri { get; set; }
}