/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task5.Threads.SharedCollection
{
    class Program
    {
        private static List<int> sharedCollection = new List<int>();
        private static TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        static void Main(string[] args)
        {
            Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
            Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
            Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
            Console.WriteLine();

            // Start tasks for adding and printing elements
            Task addingTask = Task.Run(() => AddElements());
            Task printingTask = Task.Run(() => PrintElements());

            // Wait for both tasks to complete
            Task.WaitAll(addingTask, printingTask);

            Console.WriteLine("All elements have been added and printed.");
            Console.ReadLine();
        }

        static void AddElements()
        {
            for (int i = 0; i < 10; i++)
            {
                lock (sharedCollection)
                {
                    sharedCollection.Add(i);
                    Console.WriteLine($"Added: {i}");
                }
                tcs.SetResult(true); // Signal that an item has been added
                Task.Delay(500).Wait(); // Simulate some work
            }
        }

        static void PrintElements()
        {
            for (int i = 0; i < 10; i++)
            {
                tcs.Task.Wait(); // Wait for the signal that an item has been added
                lock (sharedCollection)
                {
                    Console.WriteLine("Current Collection: " + string.Join(", ", sharedCollection));
                }
                tcs = new TaskCompletionSource<bool>(); // Reset for the next item
            }
        }
    }
}