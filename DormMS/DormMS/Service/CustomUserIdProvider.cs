using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace DormMS.Service
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            // Lấy UserId từ JWT token
            return connection.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
