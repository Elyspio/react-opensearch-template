using Mapster;
using OpenSearch.Api.Abstractions.Assemblers;
using OpenSearch.Api.Abstractions.Extensions;
using OpenSearch.Api.Abstractions.Models;
using OpenSearch.Api.Abstractions.Transports;

namespace OpenSearch.Api.Core.Assemblers;

public class ConversationAssembler : BaseAssembler<Conversation, ConversationEntity>
{
	public override Conversation Convert(ConversationEntity obj)
	{
		return obj.Adapt<Conversation>();
	}

	public override ConversationEntity Convert(Conversation obj)
	{
		return obj.Adapt<ConversationEntity>();
	}
}