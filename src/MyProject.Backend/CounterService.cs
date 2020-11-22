using MyProject.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.Backend
{
    public class CounterService : ICounterService
    {
        private int counter;

        public EventHandler<int> OnNewValue{ get; set; }

        public Task Increment(int byHowMuch)
        {
            var newValue = Interlocked.Add(ref counter, byHowMuch);
            OnNewValue?.Invoke(this, newValue);
            return Task.CompletedTask;
        }
    }
}
