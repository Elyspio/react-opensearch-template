namespace OpenSearch.Api.Abstractions.Interfaces.Hubs;

public interface IUpdateHub
{
	Task ConversationUpdated(Guid idConversation);
	Task ConversationDeleted(Guid idConversation);
}