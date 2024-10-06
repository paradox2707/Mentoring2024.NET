/*
 * 4.	Write a program which recursively creates 10 threads.
 * Each thread should be with the same body and receive a state with integer number, decrement it,
 * print and pass as a state into the newly created thread.
 * Use Thread class for this task and Join for waiting threads.
 * 
 * Implement all of the following options:
 * - a) Use Thread class for this task and Join for waiting threads.
 * - b) ThreadPool class for this task and Semaphore for waiting threads.
 */

using System;
using System.Threading;

namespace MultiThreading.Task4.Threads.Join
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("4.	Write a program which recursively creates 10 threads.");
            Console.WriteLine("Each thread should be with the same body and receive a state with integer number, decrement it, print and pass as a state into the newly created thread.");
            Console.WriteLine("Implement all of the following options:");
            Console.WriteLine();
            Console.WriteLine("- a) Use Thread class for this task and Join for waiting threads.");
            Console.WriteLine("- b) ThreadPool class for this task and Semaphore for waiting threads.");

            Console.WriteLine();

            // Part A
            Console.WriteLine("Starting Part A: Thread class with Join");
            CreateThreads(10, 10);

            // Part B
            Console.WriteLine("\nStarting Part B: ThreadPool class with Semaphore");
            CreateThreadsWithThreadPool(10, 10);

            Console.ReadLine();
        }

        // Part A: Using Thread class
        static void CreateThreads(int maxThreads, int state)
        {
            if (maxThreads <= 0) return;

            Thread thread = new Thread(() =>
            {
                Console.WriteLine($"Thread with state: {state}");
                CreateThreads(maxThreads - 1, state - 1);
            });

            thread.Start();
            thread.Join(); // Wait for the thread to finish
        }

        // Part B: Using ThreadPool and Semaphore
        static Semaphore semaphore = new Semaphore(0, 10);

        static void CreateThreadsWithThreadPool(int maxThreads, int state)
        {
            if (maxThreads <= 0) return;

            ThreadPool.QueueUserWorkItem(_ =>
            {
                Console.WriteLine($"ThreadPool thread with state: {state}");
                CreateThreadsWithThreadPool(maxThreads - 1, state - 1);
                semaphore.Release();
            });

            semaphore.WaitOne();
        }
    }
}
