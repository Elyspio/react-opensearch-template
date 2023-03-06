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
	}

	public async Task<List<Guid>> Search(string content)
	{
		var logger = _logger.Enter(Log.F(content));

		var d = new MultiSearchDescriptor();

		d.Search<ConversationIndex>(s => s
			.Index(conversationIndex)
			.Query(q => q
				.Term(m => m
					.Field(conv => conv.Title)
					.Value(content)
				)
			)
		);

		d.Search<ConversationIndex>(s => s
			.Index(conversationIndex)
			.Query(q => q
				.Nested(n => n
					.Path(conv => conv.Members)
					.Query(nq => nq
						.Bool(b => b
							.Must(must => must
								.Term(match => match
									.Field(conv => conv.Members.First().Name)
									.Value(content)
								)
							)
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

		var conversations = responses.GetResponse<ConversationIndex>(conversationIndex);
		var messages = responses.GetResponse<MessageIndex>(messagesIndex);

		var ids = new List<Guid>();

		ids.AddRange(conversations.Hits.Select(conv => conv.Source.Id));
		ids.AddRange(messages.Hits.Select(message => message.Source.IdConversation));

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

		await _client.IndexAsync(index, i => i.Index("conversations"));

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
}