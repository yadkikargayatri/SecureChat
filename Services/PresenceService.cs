using Microsoft.EntityFrameworkCore.Query.Internal;

public class PresenceService
{
    private readonly Dictionary<string, string> _online =new();
    public void SetOnline(string userId, string connectionId) => _online[userId] = connectionId;
    public void SetOffline(string userId) => _online.Remove(userId);
    public IEnumerable<string> GetOnlineUsers() => _online.Keys;
    
}