using OpenSearch.Api.Abstractions.Transports;

namespace OpenSearch.Api.Abstractions.Models;

public class ConversationIndex
{
	public required Guid Id { get; set; }
	public required string Title { get; set; }
	public required List<User> Members { get; init; }
}