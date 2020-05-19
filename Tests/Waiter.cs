using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;

namespace Tests
{
    public static class Waiter
    {
        public static void Wait<T>(Func<T> func, T expectedValue, int delayMilliseconds, TimeSpan timeout)
        {
            var sw = new Stopwatch();
            while (sw.Elapsed < timeout)
            {
                if (Equals(func.Invoke(), expectedValue))
                    return;
                Thread.Sleep(delayMilliseconds);
            }
            Assert.Fail($"Func isn't competed after {timeout.TotalSeconds} seconds");
        }
    }
}