/*
 * 1.	Write a program, which creates an array of 100 Tasks, runs them and waits all of them are finished.
 * Each Task should iterate from 1 to 1000 and print into the console the following string:
 * “Task #0 – {iteration number}”.
 */
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task1._100Tasks
{
    class Program
    {
        const int TaskAmount = 100;
        const int MaxIterationsCount = 1000;

        static void Main(string[] args)
        {
            Console.WriteLine(".Net Mentoring Program. Multi threading V1.");
            Console.WriteLine("1.	Write a program, which creates an array of 100 Tasks, runs them and waits all of them are finished.");
            Console.WriteLine("Each Task should iterate from 1 to 1000 and print into the console the following string:");
            Console.WriteLine("“Task #0 – {iteration number}”.");
            Console.WriteLine();

            StartHundredTasksBySignal();
            StartHundredTasksInParallel();

            Console.ReadLine();
        }

        private static ManualResetEvent mre = new ManualResetEvent(false);

        static void StartHundredTasksBySignal()
        {
            Task[] tasks = new Task[TaskAmount];

            for (int i = 0; i < TaskAmount; i++)
            {
                int taskNumber = i;
                tasks[i] = Task.Run(() =>
                {
                    mre.WaitOne();
                    for (int j = 1; j <= MaxIterationsCount; j++)
                    {
                        Output(taskNumber, j);
                    }
                });
            }

            mre.Set();

            Task.WaitAll(tasks);
        }

        static void StartHundredTasksInParallel()
        {
            Task[] tasks = new Task[TaskAmount];

            for (int i = 0; i < TaskAmount; i++)
            {
                int taskNumber = i;
                tasks[i] = new Task(() =>
                {
                    for (int j = 1; j <= MaxIterationsCount; j++)
                    {
                        Output(taskNumber, j);
                    }
                });
            }

            tasks.AsParallel().ForAll(t => t.Start());

            Task.WaitAll(tasks);
        }

        static void Output(int taskNumber, int iterationNumber)
        {
            Console.WriteLine($"Task #{taskNumber} – {iterationNumber}");
        }
    }
}