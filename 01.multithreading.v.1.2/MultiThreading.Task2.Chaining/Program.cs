/*
 * 2.	Write a program, which creates a chain of four Tasks.
 * First Task – creates an array of 10 random integer.
 * Second Task – multiplies this array with another random integer.
 * Third Task – sorts this array by ascending.
 * Fourth Task – calculates the average value. All this tasks should print the values to console.
 */
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task2.Chaining
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(".Net Mentoring Program. MultiThreading V1 ");
            Console.WriteLine("2.	Write a program, which creates a chain of four Tasks.");
            Console.WriteLine("First Task – creates an array of 10 random integer.");
            Console.WriteLine("Second Task – multiplies this array with another random integer.");
            Console.WriteLine("Third Task – sorts this array by ascending.");
            Console.WriteLine("Fourth Task – calculates the average value. All this tasks should print the values to console");
            Console.WriteLine();

            // Start the chain of tasks
            ExecuteTaskChain();

            Console.ReadLine();
        }

        private static ThreadLocal<Random> threadLocalRandom = new ThreadLocal<Random>(() => new Random());

        static void ExecuteTaskChain()
        {

            // First Task: Create an array of 10 random integers
            Task<int[]> firstTask = Task.Run(() =>
            {
                int[] array = new int[10];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = threadLocalRandom.Value.Next(1, 101);
                }
                Console.WriteLine("First Task - Random Array: " + string.Join(", ", array));
                return array;
            });

            // Second Task: Multiply this array with another random integer
            Task<int[]> secondTask = firstTask.ContinueWith(t =>
            {
                int multiplier = threadLocalRandom.Value.Next(1, 11);
                int[] result = t.Result.Select(x => x * multiplier).ToArray();
                Console.WriteLine($"Second Task - Multiplied by {multiplier}: " + string.Join(", ", result));
                return result;
            });

            // Third Task: Sort this array by ascending
            Task<int[]> thirdTask = secondTask.ContinueWith(t =>
            {
                int[] sorted = t.Result.OrderBy(x => x).ToArray();
                Console.WriteLine("Third Task - Sorted Array: " + string.Join(", ", sorted));
                return sorted;
            });

            // Fourth Task: Calculate the average value
            thirdTask.ContinueWith(t =>
            {
                double avg = t.Result.Average();
                Console.WriteLine($"Fourth Task - Average Value: {avg}");
            });
        }
    }
}