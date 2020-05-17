using System;
using System.Threading;
using NUnit.Framework;

namespace Tests
{
    public static class Waiter
    {
        public static void Wait<T>(Func<T> func, T expectedValue, int delayMilliseconds, TimeSpan timeout)
        {
            var retryUntil = DateTime.Now + timeout;
            while (DateTime.Now <= retryUntil)
            {
                if (Equals(func.Invoke(), expectedValue))
                    return;
                Thread.Sleep(delayMilliseconds);
            }

            Assert.Fail($"Func isn't competed after {timeout.TotalSeconds} seconds");
        }
    }
}