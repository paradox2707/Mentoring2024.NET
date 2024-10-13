using System;
using System.Threading;

namespace AsyncAwait.Task1.CancellationTokens;

internal static class Calculator
{
    public static long CalculateAsync(int n, CancellationToken token)
    {
        long sum = 0;

        for (var i = 0; i < n; i++)
        {
            Console.WriteLine($"n ={n}, ManagedThreadId - {Thread.CurrentThread.ManagedThreadId}");
            token.ThrowIfCancellationRequested(); // Check for cancellation

            sum += (i + 1);
            Thread.Sleep(6000);
        }

        return sum;
    }
}