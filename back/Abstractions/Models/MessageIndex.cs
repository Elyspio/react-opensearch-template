using OpenSearch.Api.Abstractions.Transports;

namespace OpenSearch.Api.Abstractions.Models;

public class MessageIndex
{
	public required Guid IdConversation { get; set; }
	public required string Content { get; init; }
	public required User Author { get; init; }
}