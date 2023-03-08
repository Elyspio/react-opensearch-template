using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using OpenSearch.Api.Abstractions.Helpers;
using OpenSearch.Api.Abstractions.Interfaces.Hubs;
using OpenSearch.Api.Abstractions.Interfaces.Repositories;
using OpenSearch.Api.Abstractions.Interfaces.Searches;
using OpenSearch.Api.Abstractions.Interfaces.Services;
using OpenSearch.Api.Abstractions.Models;
using OpenSearch.Api.Abstractions.Transports;
using OpenSearch.Api.Core.Assemblers;
using OpenSearch.Api.Sockets.Hubs;

namespace OpenSearch.Api.Core.Services;

public class ConversationService : IConversationService
{
	private readonly ConversationAssembler _conversationAssembler = new();
	private readonly IConversationRepository _conversationRepository;
	private readonly IConversationSearch _conversationSearch;
	private readonly IHubContext<UpdateHub, IUpdateHub> _hubContext;
	private readonly ILogger<ConversationService> _logger;

	public ConversationService(IConversationRepository conversationRepository, ILogger<ConversationService> logger, IHubContext<UpdateHub, IUpdateHub> hubContext,
		IConversationSearch conversationSearch)
	{
		_conversationRepository = conversationRepository;
		_logger = logger;
		_hubContext = hubContext;
		_conversationSearch = conversationSearch;
	}

	public async Task<List<Conversation>> Search(string? search)
	{
		var logger = _logger.Enter($"{Log.F(search)}");

		List<ConversationEntity> entities;

		if (!string.IsNullOrWhiteSpace(search))
		{
			var ids = await _conversationSearch.Search(search);
			entities = await _conversationRepository.GetByIds(ids);
		}
		else
		{
			entities = await _conversationRepository.GetAll();
		}

		var convs = _conversationAssembler.Convert(entities);

		logger.Exit();

		return convs;
	}

	public async Task<Conversation> Create(string title, List<string> members)
	{
		var logger = _logger.Enter($"{Log.F(title)} {Log.F(members)}");

		var entity = await _conversationRepository.Create(title, members);
		var conv = _conversationAssembler.Convert(entity);

		await _conversationSearch.Create(conv.Id, conv.Title, conv.Members);

		logger.Exit();

		return conv;
	}

	public async Task AddMessage(Guid id, string content, string author)
	{
		var logger = _logger.Enter($"{Log.F(id)} {Log.F(content)} {Log.F(author)}");

		var message = await _conversationRepository.AddMessage(id, content, author);

		await _conversationSearch.AddMessage(id, message);

		await _hubContext.Clients.All.ConversationUpdated(id);

		logger.Exit();
	}

	public async Task Delete(Guid id)
	{
		var logger = _logger.Enter($"{Log.F(id)}");

		await _conversationRepository.Delete(id);
		await _conversationSearch.Delete(id);
		await _hubContext.Clients.All.ConversationDeleted(id);

		logger.Exit();
	}

	public async Task Rename(Guid id, string title)
	{
		var logger = _logger.Enter($"{Log.F(id)}");

		await _conversationRepository.Rename(id, title);
		await _conversationSearch.Rename(id, title);

		await _hubContext.Clients.All.ConversationUpdated(id);

		logger.Exit();
	}

	public async Task<Conversation> GetById(Guid id)
	{
		var logger = _logger.Enter($"{Log.F(id)}");

		var entities = await _conversationRepository.GetByIds(new()
		{
			id
		});

		var conv = _conversationAssembler.Convert(entities.First());

		logger.Exit();

		return conv;
	}

	public async Task ReIndex()
	{
		var logger = _logger.Enter();

		var conversations = await Search("");

		logger.Info($"{Log.F(conversations.Count)}");

		await _conversationSearch.ReIndex(conversations);

		logger.Exit();
	}
}