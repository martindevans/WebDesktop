using System;
using System.Threading;
using System.Threading.Tasks;
using Nancy;

namespace TransparentWindow.Nancy.Modules
{
    public abstract class BaseModule
        : NancyModule
    {
        protected BaseModule()
        {
        }

        protected BaseModule(string path)
            : base(path)
        {
        }

        #region construct async handlers
        protected static Func<dynamic, CancellationToken, Task<T>> Async<T>(Func<dynamic, T> method)
        {
            return (p, c) => Task<T>.Factory.StartNew(method, p, c);
        }

        protected static Func<dynamic, CancellationToken, Task<T>> Async<T>(Func<dynamic, T> method, TaskScheduler scheduler = null)
        {
            return (p, c) => Task<T>.Factory.StartNew(method, p, c, TaskCreationOptions.PreferFairness, scheduler);
        }

        protected static Func<dynamic, CancellationToken, Task<T>> Async<T>(Func<dynamic, CancellationToken, T> method)
        {
            return (p, c) => Task<T>.Factory.StartNew(() => method(p, c), c);
        }

        protected static Func<dynamic, CancellationToken, Task<T>> Async<T>(Func<dynamic, CancellationToken, T> method, TaskScheduler scheduler = null)
        {
            return (p, c) => Task<T>.Factory.StartNew(() => method(p, c), c, TaskCreationOptions.PreferFairness, scheduler);
        }
        #endregion
    }
}
