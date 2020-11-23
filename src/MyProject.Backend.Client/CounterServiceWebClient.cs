using Microsoft.AspNetCore.SignalR.Client;
using MyProject.Common;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyProject.Backend.Client
{
    public class CounterServiceWebClient : ICounterService
    {
        private HubConnection hubConnection;
        private readonly HttpClient client;

        public EventHandler<int> OnNewValue { get ; set ; }

        public CounterServiceWebClient(HttpClient client)
        {
            this.client = client;
        }

        public async Task Increment(int byHowMuch)
        {
            if (hubConnection is null || hubConnection.State == HubConnectionState.Disconnected)
                await Connect();
            await hubConnection.SendAsync("Increment", byHowMuch);
        }

        private async Task Connect()
        {
            if (hubConnection is null)
            {
                var uri = new Uri(client.BaseAddress, "/api/hubs/counter");
                hubConnection = new HubConnectionBuilder()
                .WithUrl(uri)
                .Build();

                hubConnection.On<int>("NewValue", newValue =>
                {
                    OnNewValue?.Invoke(this, newValue);
                });
            }

            await hubConnection.StartAsync();
        }
    }
}
