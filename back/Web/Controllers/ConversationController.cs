using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using OpenSearch.Api.Abstractions.Interfaces.Services;
using OpenSearch.Api.Abstractions.Transports;
using OpenSearch.Api.Abstractions.Transports.Requests;
using System.Net;

namespace OpenSearch.Api.Web.Controllers;

[Route("api/conversations")]
[ApiController]
public class ConversationController : ControllerBase
{
	private readonly IConversationService _conversationService;

	public ConversationController(IConversationService conversationService)
	{
		_conversationService = conversationService;
	}

	[HttpGet("")]
	[SwaggerResponse(HttpStatusCode.OK, typeof(List<Conversation>))]
	public async Task<IActionResult> Search(string? content = null)
	{
		return Ok(await _conversationService.Search(content));
	}

	[HttpGet("{id}")]
	[SwaggerResponse(HttpStatusCode.OK, typeof(Conversation))]
	public async Task<IActionResult> GetById(Guid id)
	{
		return Ok(await _conversationService.GetById(id));
	}


	[HttpPost("{id:guid}/message")]
	[SwaggerResponse(HttpStatusCode.NoContent, typeof(void))]
	public async Task<IActionResult> AddMessage(Guid id, AddMessageRequest request)
	{
		await _conversationService.AddMessage(id, request.Content, request.Author);
		return NoContent();
	}

	[HttpPost("")]
	[SwaggerResponse(HttpStatusCode.Created, typeof(Conversation))]
	public async Task<IActionResult> Create(CreateConversationRequest request)
	{
		var conv = await _conversationService.Create(request.Title, request.Members);
		return Created(Request.GetDisplayUrl() + "/" + conv.Id, conv);
	}

	[HttpPut("{id:guid}/title")]
	[SwaggerResponse(HttpStatusCode.NoContent, typeof(void))]
	public async Task<IActionResult> Rename(Guid id, [FromBody] string title)
	{
		await _conversationService.Rename(id, title);
		return NoContent();
	}

	[HttpDelete("{id:guid}")]
	[SwaggerResponse(HttpStatusCode.NoContent, typeof(void))]
	public async Task<IActionResult> Delete(Guid id)
	{
		await _conversationService.Delete(id);
		return NoContent();
	}
}