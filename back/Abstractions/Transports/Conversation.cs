using System.ComponentModel.DataAnnotations;

namespace OpenSearch.Api.Abstractions.Transports;

public class Conversation : ConversationBase
{
	[Required] public required Guid Id { get; init; }
}