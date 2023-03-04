using Example.Api.Abstractions.Interfaces.Services;
using Example.Api.Abstractions.Transports;
using Example.Api.Adapters.AuthenticationApi;
using Example.Api.Web.Filters;
using Example.Api.Web.Utils;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.Net;

namespace Example.Api.Web.Controllers;

[Route("api/todo/user")]
[ApiController]
[Authorize(AuthenticationRoles.User)]
public class TodoUserController : ControllerBase
{
	private readonly ITodoService todoService;

	public TodoUserController(ITodoService todoService)
	{
		this.todoService = todoService;
	}

	[HttpDelete("{id:guid}")]
	[SwaggerResponse(HttpStatusCode.NoContent, typeof(void))]
	public async Task<IActionResult> DeleteForUser(Guid id)
	{
		await todoService.DeleteForUser(id, AuthUtility.GetUser(Request).Username);
		return NoContent();
	}

	[HttpPost]
	[SwaggerResponse(HttpStatusCode.Created, typeof(Todo))]
	public async Task<IActionResult> AddForUser([FromBody] string label)
	{
		var todo = await todoService.AddForUser(label, AuthUtility.GetUser(Request).Username);
		return Created($"/{todo.Id}", todo);
	}


	[HttpGet]
	[SwaggerResponse(HttpStatusCode.OK, typeof(List<Todo>))]
	public async Task<IActionResult> GetAllForUser()
	{
		var user = AuthUtility.GetUser(Request).Username;
		return Ok(await todoService.GetAllForUser(user));
	}


	[HttpPut("{id:guid}/toggle")]
	[SwaggerResponse(HttpStatusCode.OK, typeof(Todo))]
	public async Task<IActionResult> CheckForUser(Guid id)
	{
		return Ok(await todoService.CheckForUser(id, AuthUtility.GetUser(Request).Username));
	}
}