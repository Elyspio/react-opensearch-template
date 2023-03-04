using OpenSearch.Api.Abstractions.Helpers;
using OpenSearch.Api.Abstractions.Interfaces.Repositories;
using OpenSearch.Api.Abstractions.Interfaces.Services;
using OpenSearch.Api.Abstractions.Transports;
using Microsoft.Extensions.Logging;
using OpenSearch.Api.Abstractions.Interfaces.Hubs;
using OpenSearch.Api.Core.Assemblers;
using OpenSearch.Api.Sockets.Hubs;
using Microsoft.AspNetCore.SignalR;
using OpenSearch.Api.Abstractions.Models;

namespace OpenSearch.Api.Core.Services;

public class ConversationService : IConversationService
{
	private readonly ILogger<ConversationService> _logger;
	private readonly IConversationRepository _conversationRepository;
	private readonly ConversationAssembler _conversationAssembler = new();
	private readonly IHubContext<UpdateHub, IUpdateHub> _hubContext;

	public ConversationService(IConversationRepository conversationRepository, ILogger<ConversationService> logger, IHubContext<UpdateHub, IUpdateHub> hubContext)
	{
		_conversationRepository = conversationRepository;
		_logger = logger;
		_hubContext = hubContext;
	}

	public async Task<List<Conversation>> Search(string? search)
	{
		var logger = _logger.Enter($"{Log.F(search)}");

		List<ConversationEntity> entities;

		if (!string.IsNullOrWhiteSpace(search))
		{
			var ids = new List<Guid>();
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

		logger.Exit();

		return conv;
	}

	public async Task AddMessage(Guid id, string content, string author)
	{
		var logger = _logger.Enter($"{Log.F(id)} {Log.F(content)} {Log.F(author)}");

		await _conversationRepository.AddMessage(id, content, author);

		await _hubContext.Clients.All.ConversationUpdated(id);

		logger.Exit();
	}

	public async Task Delete(Guid id)
	{
		var logger = _logger.Enter($"{Log.F(id)}");

		await _conversationRepository.Delete(id);

		await _hubContext.Clients.All.ConversationDeleted(id);

		logger.Exit();
	}

	public async Task Rename(Guid id, string title)
	{
		var logger = _logger.Enter($"{Log.F(id)}");

		await _conversationRepository.Rename(id, title);
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
}