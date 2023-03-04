using Example.Api.Abstractions.Helpers;
using Example.Api.Abstractions.Interfaces.Repositories;
using Example.Api.Abstractions.Interfaces.Services;
using Example.Api.Abstractions.Transports;
using Example.Api.Core.Assemblers;
using Microsoft.Extensions.Logging;

namespace Example.Api.Core.Services;

public class TodoService : ITodoService
{
	private readonly ILogger<TodoService> _logger;
	private readonly ITodoRepository _todoRepository;
	private readonly string defaultUser = "public";
	private readonly TodoAssembler todoAssembler = new();

	public TodoService(ITodoRepository todoRepository, ILogger<TodoService> logger)
	{
		_todoRepository = todoRepository;
		_logger = logger;
	}

	public async Task<Todo> Add(string label)
	{
		var logger = _logger.Enter(Log.Format(label));

		var entity = await _todoRepository.Add(label, defaultUser);
		var data = todoAssembler.Convert(entity);

		logger.Exit();

		return data;
	}

	public async Task<Todo> AddForUser(string label, string user)
	{
		var logger = _logger.Enter($"{Log.Format(user)} {Log.Format(label)}");

		var entity = await _todoRepository.Add(label, user);
		var data = todoAssembler.Convert(entity);

		logger.Exit();

		return data;
	}

	public async Task<List<Todo>> GetAllForUser(string user)
	{
		var logger = _logger.Enter($"{Log.Format(user)}");

		var entities = await _todoRepository.GetAll(user);
		var data = todoAssembler.Convert(entities);

		logger.Exit();

		return data;
	}

	public async Task<List<Todo>> GetAll()
	{
		var logger = _logger.Enter();

		var entities = await _todoRepository.GetAll(defaultUser);
		var data = todoAssembler.Convert(entities);

		logger.Exit();

		return data;
	}

	public async Task<Todo> Check(Guid id)
	{
		var logger = _logger.Enter(Log.Format(id));

		var entity = await _todoRepository.Check(id, defaultUser);
		var data = todoAssembler.Convert(entity);

		logger.Exit();

		return data;
	}

	public async Task<Todo> CheckForUser(Guid id, string user)
	{
		var logger = _logger.Enter($"{Log.Format(user)} {Log.Format(id)}");

		var entity = await _todoRepository.Check(id, user);
		var data = todoAssembler.Convert(entity);

		logger.Exit();

		return data;
	}

	public async Task Delete(Guid id)
	{
		var logger = _logger.Enter(Log.Format(id));

		await _todoRepository.Delete(id, defaultUser);

		logger.Exit();
	}

	public async Task DeleteForUser(Guid id, string user)
	{
		var logger = _logger.Enter(Log.Format(id));

		await _todoRepository.Delete(id, defaultUser);

		logger.Exit();
	}
}