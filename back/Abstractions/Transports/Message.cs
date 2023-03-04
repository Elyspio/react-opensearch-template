namespace OpenSearch.Api.Abstractions.Transports;

public class Message
{
	public required string Content { get; init; }
	public required User Author { get; init; }
	public required DateTime Created { get; init; }
}