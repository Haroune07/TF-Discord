using System;
using System.Threading.Tasks;

namespace Frontend.Services
{
    public interface IDispatcherService
    {
        void Invoke(Action action);
        Task InvokeAsync(Action action);
    }
}
