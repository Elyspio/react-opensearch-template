namespace OpenSearch.Api.Web.Controllers;

public class CreateConversationRequest		
{
	public required string Title { get; init; }
	public required List<string> Members { get; init; }
}