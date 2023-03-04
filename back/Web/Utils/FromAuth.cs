using Example.Api.Adapters.AuthenticationApi;

namespace Example.Api.Web.Utils;

public class AuthUtility
{
	public static User GetUser(HttpRequest request)
	{
		return (User) request.HttpContext.Items["user"];
	}

	public static string GetToken(HttpRequest request)
	{
		return request.Headers.Authorization;
	}
}