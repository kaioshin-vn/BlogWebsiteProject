using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.Http;

namespace Client.Hub
{
    public static class Notice
    {
        public static async Task RaiseNotice (Guid userId, IHttpClientFactory HttpClientFactory, NavigationManager Navigation)
        {
            var hubNotice = new HubConnectionBuilder()
            .WithUrl(Navigation.ToAbsoluteUri("/hubNotice"))
            .Build();

            await hubNotice.StartAsync();

            var httpClient = HttpClientFactory.CreateClient("MyHttpClient");
            var response = await httpClient.GetStringAsync($"/getAmountNoticeUser/{userId}");
            var NoticeCount = 0;
            if (response != null)
            {
                 NoticeCount = Convert.ToInt32(response);
            }
            await hubNotice.SendAsync("SendPrivateMessage", userId, NoticeCount);

            await hubNotice.StopAsync();
            await hubNotice.DisposeAsync();
        }
    }
}
