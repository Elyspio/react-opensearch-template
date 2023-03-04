using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OpenSearch.Api.Abstractions.Transports;

namespace OpenSearch.Api.Abstractions.Models;

public class ConversationEntity : ConversationBase
{
	[BsonId]
	[BsonRepresentation(BsonType.ObjectId)]
	public ObjectId Id { get; init; }
}