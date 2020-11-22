using Microsoft.AspNetCore.SignalR;
using MyProject.Common;
using System.Threading.Tasks;

namespace MyProject.Backend.Controller.Hubs
{
    public class CounterHub : Hub
    {
        private readonly ICounterService counterService;

        public CounterHub( ICounterService counterService)
        {
            counterService.OnNewValue += OnNewValue;
            this.counterService = counterService;
        }

        private async void OnNewValue(object sender, int newValue)
        {
            await Clients.All.SendAsync("NewValue", newValue);
        }

        public async Task Increment(int byHowMuch)
        {
            await counterService.Increment(byHowMuch);
        }
    }
}
