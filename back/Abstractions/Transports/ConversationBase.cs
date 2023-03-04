namespace OpenSearch.Api.Abstractions.Transports;

public class ConversationBase
{
	public required string Title { get; set; }
	public required List<User> Members { get; init; }
	public required List<Message> Messages { get; init; }
}