using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SecureChat.Hubs
{
    public class NameIdentifierUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.Identity?.Name;
        }
    }
}