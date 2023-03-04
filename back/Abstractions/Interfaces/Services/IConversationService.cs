using OpenSearch.Api.Abstractions.Transports;

namespace OpenSearch.Api.Abstractions.Interfaces.Services;

public interface IConversationService
{
	Task<List<Conversation>> Search(string? search);
	Task<Conversation> Create(string title, List<string> members);
	Task AddMessage(Guid id, string content, string author);
	Task Delete(Guid id);
	Task Rename(Guid id, string title);
	Task<Conversation> GetById(Guid id);
}