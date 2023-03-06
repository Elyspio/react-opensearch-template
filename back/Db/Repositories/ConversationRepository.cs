using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using OpenSearch.Api.Abstractions.Extensions;
using OpenSearch.Api.Abstractions.Helpers;
using OpenSearch.Api.Abstractions.Interfaces.Repositories;
using OpenSearch.Api.Abstractions.Models;
using OpenSearch.Api.Abstractions.Transports;
using OpenSearch.Api.Db.Repositories.Internal;

namespace OpenSearch.Api.Db.Repositories;

internal class ConversationRepository : BaseRepository<ConversationEntity>, IConversationRepository
{
	private readonly ILogger<ConversationRepository> _logger;

	public ConversationRepository(IConfiguration configuration, ILogger<BaseRepository<ConversationEntity>> baseLogger, ILogger<ConversationRepository> logger) : base(configuration, baseLogger)
	{
		_logger = logger;
	}

	public async Task<ConversationEntity> Create(string title, List<string> members)
	{
		var logger = _logger.Enter($"{Log.F(title)} {Log.F(members)}");

		var conv = new ConversationEntity
		{
			Members = members.Select(member => new User
			{
				Name = member
			}).ToList(),
			Messages = new(),
			Title = title
		};

		await EntityCollection.InsertOneAsync(conv);

		logger.Exit();

		return conv;
	}

	public async Task<Message> AddMessage(Guid id, string content, string author)
	{
		var logger = _logger.Enter($"{Log.F(id)} {Log.F(content)} {Log.F(author)}");

		var message = new Message
		{
			Author = new()
			{
				Name = author
			},
			Content = content,
			Created = DateTime.Now
		};
		var update = Builders<ConversationEntity>.Update.Push(conv => conv.Messages, message);

		await EntityCollection.UpdateOneAsync(conv => conv.Id == id.AsObjectId(), update);

		logger.Exit();

		return message;
	}

	public async Task<List<ConversationEntity>> GetByIds(List<Guid> ids)
	{
		var logger = _logger.Enter(Log.F(ids));

		var objectIds = ids.Select(id => id.AsObjectId());
		var convs = await EntityCollection.AsQueryable().Where(conv => objectIds.Contains(conv.Id)).ToListAsync();

		logger.Exit();

		return convs;
	}

	public async Task<List<ConversationEntity>> GetAll()
	{
		var logger = _logger.Enter();

		var convs = await EntityCollection.AsQueryable().ToListAsync();

		logger.Exit();

		return convs;
	}

	public async Task Delete(Guid id)
	{
		var logger = _logger.Enter(Log.F(id));

		await EntityCollection.DeleteOneAsync(conv => conv.Id == id.AsObjectId());

		logger.Exit();
	}

	public async Task Rename(Guid id, string title)
	{
		var logger = _logger.Enter($"{Log.Format(id)} {Log.F(title)}");

		var update = Builders<ConversationEntity>.Update.Set(conv => conv.Title, title);

		await EntityCollection.UpdateOneAsync(conv => conv.Id == id.AsObjectId(), update);

		logger.Exit();
	}
}