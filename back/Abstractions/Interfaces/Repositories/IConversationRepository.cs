using OpenSearch.Api.Abstractions.Models;
using OpenSearch.Api.Abstractions.Transports;

namespace OpenSearch.Api.Abstractions.Interfaces.Repositories;

public interface IConversationRepository
{
	Task<ConversationEntity> Create(string title, List<string> members);
	Task AddMessage(Guid id, string content, string author);
	Task<List<ConversationEntity>> GetByIds(List<Guid> ids);
	Task<List<ConversationEntity>> GetAll();
	Task Delete(Guid id);
	Task Rename(Guid id, string title);

}