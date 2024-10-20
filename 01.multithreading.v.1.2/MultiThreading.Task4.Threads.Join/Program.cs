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
            Console.WriteLine("4. Write a program which recursively creates 10 threads.");
            Console.WriteLine("Each thread should be with the same body and receive a state with integer number, decrement it, print and pass as a state into the newly created thread.");
            Console.WriteLine("Implement all of the following options:");
            Console.WriteLine();
            Console.WriteLine("- a) Use Thread class for this task and Join for waiting threads.");
            Console.WriteLine("- b) ThreadPool class for this task and Semaphore for waiting threads.");

            Console.WriteLine();

            // Part A
            Console.WriteLine("Starting Part A: Thread class with Join");
            CreateThread(10);

            // Part B
            Console.WriteLine("\nStarting Part B: ThreadPool class with Semaphore");
            CreateThreadsWithThreadPool(10);

            Console.ReadLine();
        }

        // Part A: Using Thread class
        static void CreateThread(object state)
        {
            int number = (int)state;
            if (number <= 0) return;
            Console.WriteLine($"Thread {number} started.");
            Thread thread = new Thread(new ParameterizedThreadStart(CreateThread));
            thread.Start(number - 1);
            thread.Join();

            Console.WriteLine($"Thread {number} finished.");
        }

        // Part B: Using ThreadPool and Semaphore
        static void CreateThreadsWithThreadPool(int number)
        {
            ThreadWithState tws = new ThreadWithState(10);
            ThreadPool.QueueUserWorkItem(tws.ThreadProc);
            Console.ReadLine();
        }
    }

    public class ThreadWithState
    {
        static Semaphore semaphore = new Semaphore(1, 1);
        private int number;

        public ThreadWithState(int number)
        {
            this.number = number;
        }

        public void ThreadProc(object state)
        {
            semaphore.WaitOne();
            Console.WriteLine($"Thread {number} started.");
            if (number > 1)
            {
                ThreadWithState newTws = new ThreadWithState(number - 1);
                ThreadPool.QueueUserWorkItem(newTws.ThreadProc);
            }
            Console.WriteLine($"Thread {number} finished.");
            semaphore.Release();
        }
    }
}
