/*
*  Create a Task and attach continuations to it according to the following criteria:
   a.    Continuation task should be executed regardless of the result of the parent task.
   b.    Continuation task should be executed when the parent task finished without success.
   c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
   d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled
   Demonstrate the work of the each case with console utility.
*/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task6.Continuation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create a Task and attach continuations to it according to the following criteria:");
            Console.WriteLine("a.    Continuation task should be executed regardless of the result of the parent task.");
            Console.WriteLine("b.    Continuation task should be executed when the parent task finished without success.");
            Console.WriteLine("c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.");
            Console.WriteLine("d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled.");
            Console.WriteLine("Demonstrate the work of the each case with console utility.");
            Console.WriteLine();

            // Scenario a
            var parentTaskA = Task.Run(() =>
            {
                Console.WriteLine($"Parent task A running on thread {Thread.CurrentThread.ManagedThreadId}, Thread Pool: {Thread.CurrentThread.IsThreadPoolThread}.");
            });

            var continuationTaskA = parentTaskA.ContinueWith(t =>
            {
                Console.WriteLine($"Continuation task A executed regardless of the result on thread {Thread.CurrentThread.ManagedThreadId}, Thread Pool: {Thread.CurrentThread.IsThreadPoolThread}.");
            }, TaskContinuationOptions.None);

            // Scenario b
            var parentTaskB = Task.Run(() =>
            {
                Console.WriteLine($"Parent task B running on thread {Thread.CurrentThread.ManagedThreadId}, Thread Pool: {Thread.CurrentThread.IsThreadPoolThread}.");
                throw new Exception("Simulated exception for B");
            });

            var continuationTaskB = parentTaskB.ContinueWith(t =>
            {
                Console.WriteLine($"Continuation task B executed because the parent task did not succeed on thread {Thread.CurrentThread.ManagedThreadId}, Thread Pool: {Thread.CurrentThread.IsThreadPoolThread}.");
            }, TaskContinuationOptions.NotOnRanToCompletion);

            // Scenario c
            var parentTaskC = Task.Run(() =>
            {
                Console.WriteLine($"Parent task C running on thread {Thread.CurrentThread.ManagedThreadId}, Thread Pool: {Thread.CurrentThread.IsThreadPoolThread}.");
                throw new Exception("Simulated exception for C");
            });

            var continuationTaskC = parentTaskC.ContinueWith(t =>
            {
                Console.WriteLine($"Continuation task C executed on the same thread {Thread.CurrentThread.ManagedThreadId} after parent task failed, Thread Pool: {Thread.CurrentThread.IsThreadPoolThread}.");
            }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);

            // Scenario d
            var ctsD = new CancellationTokenSource();
            var parentTaskD = Task.Run(() =>
            {
                Console.WriteLine($"Parent task D running on thread {Thread.CurrentThread.ManagedThreadId}, Thread Pool: {Thread.CurrentThread.IsThreadPoolThread}.");
                ctsD.Cancel();
                throw new OperationCanceledException(ctsD.Token);
            }, ctsD.Token);

            var continuationTaskD = parentTaskD.ContinueWith(t =>
            {
                Console.WriteLine($"Continuation task D executed outside of the thread pool on thread {Thread.CurrentThread.ManagedThreadId}, Thread Pool: {Thread.CurrentThread.IsThreadPoolThread} after parent task was cancelled.");
            }, TaskContinuationOptions.OnlyOnCanceled | TaskContinuationOptions.LongRunning);

            // Wait for all tasks to complete
            Task.WaitAll(new Task[] { continuationTaskA, continuationTaskB, continuationTaskC, continuationTaskD });

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }
    }
}