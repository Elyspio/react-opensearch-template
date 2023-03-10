using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenSearch.Api.Abstractions.Helpers;
using OpenSearch.Api.Abstractions.Interfaces.Searches;
using OpenSearch.Api.Abstractions.Models;
using OpenSearch.Api.Abstractions.Transports;
using OpenSearch.Api.Search.Searches.Internal;
using OpenSearch.Client;

namespace OpenSearch.Api.Search.Searches;

public class ConversationSearch : BaseSearchEngine, IConversationSearch
{
	private const string conversationIndex = "conversations";
	private const string messagesIndex = "messages";

	private readonly ILogger<ConversationSearch> _logger;

	public ConversationSearch(IConfiguration configuration, ILogger<ConversationSearch> logger) : base(configuration)
	{
		_logger = logger;

		CreateIndex().GetAwaiter().GetResult();
	}

	public async Task<List<Guid>> Search(string content)
	{
		var logger = _logger.Enter(Log.F(content));

		var d = new MultiSearchDescriptor();

		d.Search<ConversationIndex>(s => s
			.Index(conversationIndex)
			.Query(q => q
				.Match(m => m
					.Field(conv => conv.Title)
					.Query(content).Fuzziness(Fuzziness.Auto)
				)
			)
		);

		d.Search<ConversationIndex>(s => s
			.Index(conversationIndex)
			.Query(q => q
				.Nested(n => n
					.Path(conv => conv.Members)
					.Query(nq => nq
						.Match(match => match
							.Field(conv => conv.Members.First().Name)
							.Query(content)
						)
					)
				)
			)
		);

		d.Search<MessageIndex>(s => s.Index(messagesIndex)
			.Query(q => q
				.Match(m => m
					.Field(message => message.Content)
					.Query(content)
				)
			)
		);


		var responses = await _client.MultiSearchAsync(d);


		var conversations = responses.GetResponses<ConversationIndex>();
		var messages = responses.GetResponses<MessageIndex>();

		var ids = new List<Guid>();

		ids.AddRange(conversations.SelectMany(x => x.Hits.Select(conv => conv.Source.Id)));
		ids.AddRange(messages.SelectMany(x => x.Hits.Select(conv => conv.Source.IdConversation)));

		logger.Exit(Log.F(ids.Count));

		return ids;
	}

	public async Task Create(Guid id, string title, List<User> members)
	{
		var logger = _logger.Enter($"{Log.F(id)} {Log.F(title)} {Log.F(members)}");

		var index = new ConversationIndex
		{
			Id = id,
			Members = members,
			Title = title
		};

		await _client.IndexAsync(index, i => i.Index(conversationIndex));

		logger.Exit();
	}

	public async Task Rename(Guid id, string title)
	{
		var logger = _logger.Enter($"{Log.F(id)} {Log.F(title)}");

		var conv = await _client.SearchAsync<ConversationIndex>(sd => sd
			.Index(conversationIndex)
			.Query(q => q
				.Term(t => t
					.Field(conv => conv.Id)
					.Value(id)
				)
			)
		);

		var idDocument = conv.Hits.First().Id;


		await _client.UpdateAsync<ConversationIndex, object>(idDocument, u => u.Index(conversationIndex).Doc(new
		{
			Title = title
		}));

		logger.Exit();
	}

	public async Task AddMessage(Guid id, Message message)
	{
		var logger = _logger.Enter($"{Log.F(id)} {Log.F(message)}");

		var index = new MessageIndex
		{
			IdConversation = id,
			Author = message.Author,
			Content = message.Content
		};

		await _client.IndexAsync(index, i => i.Index(messagesIndex));

		logger.Exit();
	}


	public async Task Delete(Guid id)
	{
		var logger = _logger.Enter(Log.F(id));

		await _client.DeleteByQueryAsync<ConversationIndex>(descriptor => descriptor
			.Index(conversationIndex)
			.Query(q => q
				.Term(t => t
					.Field(conv => conv.Id)
					.Value(id)
				)
			)
		);


		await _client.DeleteByQueryAsync<MessageIndex>(descriptor => descriptor
			.Index(messagesIndex)
			.Query(q => q
				.Term(m => m
					.Field(conv => conv.IdConversation)
					.Value(id)
				)
			)
		);


		logger.Exit();
	}

	public async Task ReIndex(List<Conversation> conversations)
	{
		await _client.Indices.DeleteAsync(conversationIndex);
		await _client.Indices.DeleteAsync(messagesIndex);

		await CreateIndex();

		await _client.IndexManyAsync(conversations.Select(conv => new ConversationIndex
		{
			Id = conv.Id,
			Members = conv.Members,
			Title = conv.Title
		}), conversationIndex);


		await _client.IndexManyAsync(conversations.SelectMany(conv => conv.Messages.Select(msg => new MessageIndex
		{
			Author = msg.Author,
			Content = msg.Content,
			IdConversation = conv.Id
		})), messagesIndex);
	}

	private async Task CreateIndex()
	{
		await CreateIndexIfMissing<MessageIndex>(messagesIndex);
		await _client.Indices.CreateAsync(conversationIndex, c => c
			.Map<ConversationIndex>(m => m
				.AutoMap()
				.Properties(ps => ps
					.Nested<User>(n => n
						.Name(p => p.Members)
					)
				)
			)
		);
	}
}