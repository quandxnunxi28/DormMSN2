using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;


namespace DormMS.Service

{
    [Authorize]
    public class NotificationHub :Hub
    {
        // Hàm này cho phép server gửi message đến 1 user cụ thể
        public async Task SendNotification(string userId, string message)
        {
            await Clients.Caller.SendAsync("ReceiveNotification", "🔥 Bạn đã kết nối SignalR!");
            await base.OnConnectedAsync();
            // Clients.User(userId) = gửi cho đúng user đang đăng nhập
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
            // "ReceiveNotification" = tên hàm phía JS sẽ nhận
        }
        public override async Task OnConnectedAsync()
        {   
            Console.WriteLine("🔥 CONNECTED!");

            var userId = Context.UserIdentifier;
            Console.WriteLine("🔥 UserId: " + userId);

            var nameId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine("🔥 Claim NameIdentifier: " + nameId);

            await base.OnConnectedAsync();
        }
    }
}
