using System;
using System.Threading;

namespace Game.Common
{
    public static class Waiter
    {
        public static bool Wait<T>(Func<T> func, T expectedValue, int delayMilliseconds, TimeSpan timeout)
        {
            var retryUntil = DateTime.Now + timeout;
            while (DateTime.Now <= retryUntil)
            {
                if (Equals(func.Invoke(), expectedValue))
                    return true;
                Thread.Sleep(delayMilliseconds);
            }

            return false;
        }
    }
}