using Microsoft.AspNetCore.SignalR;
using OpenSearch.Api.Abstractions.Interfaces.Hubs;

namespace OpenSearch.Api.Sockets.Hubs;

public class UpdateHub : Hub<IUpdateHub>
{
}