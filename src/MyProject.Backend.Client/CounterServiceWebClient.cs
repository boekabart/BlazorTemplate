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

        public EventHandler<int> OnNewValue { get ; set ; }

        public CounterServiceWebClient(HttpClient client)
        {
            var uri = new Uri(client.BaseAddress, "hubs/counter");
            hubConnection = new HubConnectionBuilder()
            .WithUrl(uri)
            .Build();

            hubConnection.On<int>("NewValue", newValue =>
            {
                OnNewValue?.Invoke(this, newValue);
            });

            hubConnection.StartAsync().Wait();
        }

        public async Task Increment(int byHowMuch)
        {
            await hubConnection.SendAsync("Increment", byHowMuch);
        }
    }
}
