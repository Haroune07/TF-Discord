using System;
using System.Threading.Tasks;
using System.Windows;

namespace Frontend.Services
{
    public class WpfDispatcherService : IDispatcherService
    {
        public void Invoke(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }

        public Task InvokeAsync(Action action)
        {
            return Application.Current.Dispatcher.InvokeAsync(action).Task;
        }
    }
}
