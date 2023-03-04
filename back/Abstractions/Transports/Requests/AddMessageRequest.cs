namespace OpenSearch.Api.Abstractions.Transports.Requests;

public class AddMessageRequest

{
	public required string Content { get; init; }
	public required string Author { get; init; }
}