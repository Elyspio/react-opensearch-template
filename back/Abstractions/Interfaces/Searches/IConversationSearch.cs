using OpenSearch.Api.Abstractions.Transports;

namespace OpenSearch.Api.Abstractions.Interfaces.Searches;

public interface IConversationSearch
{
	public Task<List<Guid>> Search(string content);

	public Task Create(Guid id, string title, List<User> members);

	public Task Rename(Guid id, string title);

	public Task AddMessage(Guid id, Message message);

	public Task Delete(Guid id);
}