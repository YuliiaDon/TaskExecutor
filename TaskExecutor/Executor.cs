using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace TaskExecutor
{
    public class Executor
    {
        private readonly ConcurrentQueue<Action> _tasksQueue = new ConcurrentQueue<Action>();
        private Thread _threadForTasks;
        private CancellationTokenSource _cancellationSource;

        /// <summary>
        /// Provides number of tasks in the queue.
        /// </summary>
        public int NumberOfTasks
        {
            get { return _tasksQueue.Count(); }
        }

        /// <summary>
        /// Adds task into the queue to be executed.
        /// </summary>
        /// <param name="task">Task to execute</param>
        public void AddToExecutionQueue(Action task)
        {
            _tasksQueue.Enqueue(task);
        }

        /// <summary>
        /// Starts the thread in which execution of task happens..
        /// </summary>
        /// <param name="token"></param>
        private void Initialize()
        {
            _cancellationSource = new CancellationTokenSource();
            _threadForTasks = new Thread(() =>
            {
                while (!_cancellationSource.Token.IsCancellationRequested)
                {
                    Action taskToExecute;
                    if (_tasksQueue.TryDequeue(out taskToExecute))
                    {
                        taskToExecute();
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
                _cancellationSource.Dispose();
            });
            _threadForTasks.Start();
        }

        /// <summary>
        /// Executor starts carrying out tasks in the queue.
        /// </summary>
        public void StartExecutor()
        {
            if (_threadForTasks == null)
            {
                Initialize();
            }
        }

        /// <summary>
        /// Stops the execution thead.
        /// </summary>
        public void StopExecutor()
        {
            _threadForTasks = null;
            _cancellationSource.Cancel();
        }
    }
}