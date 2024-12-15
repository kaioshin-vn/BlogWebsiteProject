
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

public class ConversationHub : Hub
{
    // Sử dụng ConcurrentDictionary để lưu trữ GUID người dùng và Connection ID
    private static ConcurrentDictionary<string, string> _userConnections = new();

    public override Task OnConnectedAsync()
    {
        // Mặc định, không có tham số nào được truyền, bạn có thể xử lý thêm tại đây nếu cần.
        return base.OnConnectedAsync();
    }

    // Phương thức này sẽ được gọi từ client để gửi GUID của người dùng lên
    public async Task RegisterConnection(string userConver)
    {
        var connectionId = Context.ConnectionId;  // Lấy Connection ID từ context
        if (!string.IsNullOrEmpty(userConver))
        {
            await Groups.AddToGroupAsync(connectionId, userConver);
        }
    }

    public override async Task OnDisconnectedAsync(System.Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        var userGuid = _userConnections.FirstOrDefault(x => x.Value == connectionId).Key;

        if (!string.IsNullOrEmpty(userGuid))
        {
            _userConnections.TryRemove(userGuid, out _);
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Phương thức gửi thông báo riêng tới một người dùng cụ thể
    public async Task SendPrivateMessage(string userConver, string Id)
    {
        if (!String.IsNullOrEmpty(userConver))
        {
            await Clients.Group(userConver).SendAsync("ReceiveMessage", Id);
        }
    }
}
