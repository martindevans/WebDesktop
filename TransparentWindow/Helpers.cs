using System;
using System.Threading;

namespace TransparentWindow
{
    public static class Helpers
    {
        public static T TryExponentialBackoff<T>(Func<T> func, int maxWait = 2500, int maxAttempts = int.MaxValue) where T : class
        {
            T result = null;
            for (int attempt = 0; attempt < maxAttempts && result == null; attempt++)
            {
                Thread.Sleep(Math.Min(maxWait, (int)Math.Pow(3, attempt)));
                result = func();
            }

            return result;
        }
    }
}
