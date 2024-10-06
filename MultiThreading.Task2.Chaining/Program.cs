/*
 * 2.	Write a program, which creates a chain of four Tasks.
 * First Task – creates an array of 10 random integer.
 * Second Task – multiplies this array with another random integer.
 * Third Task – sorts this array by ascending.
 * Fourth Task – calculates the average value. All this tasks should print the values to console.
 */
using System;
using System.Linq;
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

        static async void ExecuteTaskChain()
        {
            // First Task: Create an array of 10 random integers
            var random = new Random();
            int[] randomArray = await Task.Run(() =>
            {
                int[] array = new int[10];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = random.Next(1, 101);
                }
                Console.WriteLine("First Task - Random Array: " + string.Join(", ", array));
                return array;
            });

            // Second Task: Multiply this array with another random integer
            int multiplier = random.Next(1, 11);
            int[] multipliedArray = await Task.Run(() =>
            {
                int[] result = randomArray.Select(x => x * multiplier).ToArray();
                Console.WriteLine($"Second Task - Multiplied by {multiplier}: " + string.Join(", ", result));
                return result;
            });

            // Third Task: Sort this array by ascending
            int[] sortedArray = await Task.Run(() =>
            {
                int[] sorted = multipliedArray.OrderBy(x => x).ToArray();
                Console.WriteLine("Third Task - Sorted Array: " + string.Join(", ", sorted));
                return sorted;
            });

            // Fourth Task: Calculate the average value
            double average = await Task.Run(() =>
            {
                double avg = sortedArray.Average();
                Console.WriteLine($"Fourth Task - Average Value: {avg}");
                return avg;
            });
        }
    }
}