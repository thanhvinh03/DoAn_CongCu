using Microsoft.AspNetCore.SignalR;

public class OrderHub : Hub
{
    // Phương thức này sẽ được gọi từ server để cập nhật danh sách món ăn
    public async Task UpdateOrderList()
    {
        await Clients.All.SendAsync("ReceiveOrderUpdate");
    }
}