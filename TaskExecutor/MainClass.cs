using System;
using System.Threading;

namespace TaskExecutor
{
    public class MainClass
    {
        static void Main(string[] args)
        {
            Executor executor = new Executor();

            //We start executor. It starts checking its execution queue and execute task if present
            Console.WriteLine("Main Thread - Starting executor");
            executor.StartExecutor();

            //Create threads, which will be adding tasks for execution
            for (int i = 0; i < 15; i++)
            {
                Thread threadForTasks = new Thread(() =>
                {
                    Action toExecute = new SomeObject(Thread.CurrentThread.ManagedThreadId).ToExecute;
                    executor.AddToExecutionQueue(toExecute);
                });
                Console.WriteLine("Task with ID {0} was added", threadForTasks.ManagedThreadId);
                threadForTasks.Start();
            }

            //Let executor execute some tasks
            Thread.Sleep(3000);

            //Stop execution. 
            //Task that was executing at this moment will stop as soon as it will be possible
            Console.WriteLine("Main Thread - Stopping executor");
            executor.StopExecutor();
            Thread.Sleep(5000);

            //Start execution again. Executor starts the next available task
            Console.WriteLine("Main Thread - Starting executor");
            executor.StartExecutor();
            Thread.Sleep(5000);

            Console.WriteLine("Main Thread - Stopping executor");
            executor.StopExecutor();
            Thread.Sleep(5000);

            Console.WriteLine("Main Thread - Starting executor");
            executor.StartExecutor();

            //While executor has tasks to do - let it work
            while (executor.NumberOfTasks > 0)
            {
                Thread.Sleep(1000);
            }
            Console.WriteLine("Main Thread - Stopping executor");
            executor.StopExecutor();
        }
        
    }
}
