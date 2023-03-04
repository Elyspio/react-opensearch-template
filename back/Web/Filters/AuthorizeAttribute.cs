using Example.Api.Abstractions.Interfaces.Services;
using Example.Api.Adapters.AuthenticationApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace Example.Api.Web.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
	private readonly AuthenticationRoles _role;


	public AuthorizeAttribute(AuthenticationRoles role)
	{
		_role = role;
	}

	/// <inheritdoc />
	public void OnAuthorization(AuthorizationFilterContext context)
	{
		var svc = context.HttpContext.RequestServices;
		var tokenService = svc.GetRequiredService<IAuthenticationService>();

		// skip authorization if action is decorated with [AllowAnonymous] attribute
		var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
		if (allowAnonymous)
			return;


		var bearer = context.HttpContext.Request.Headers.Authorization.ToString();

		if (!tokenService.ValidateJwt(bearer, out var token))
		{
			context.Result = new JsonResult(new
			{
				status = "Unauthorized"
			})
			{
				StatusCode = StatusCodes.Status401Unauthorized
			};
			return;
		}


		var userStr = token!.Payload["data"].ToString()!;

		var user = JsonConvert.DeserializeObject<User>(userStr);

		context.HttpContext.Items["user"] = user;

		if (user.Authorizations.Authentication.Roles.Contains(_role) != true)
			context.Result = new JsonResult(new
			{
				status = "Forbidden",
				missingRole = _role
			})
			{
				StatusCode = StatusCodes.Status403Forbidden
			};
	}
}