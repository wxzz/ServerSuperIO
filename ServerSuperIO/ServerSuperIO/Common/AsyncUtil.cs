using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSuperIO.Common
{
    /// <summary>
    /// Async extension class
    /// </summary>
    public static class AsyncUtil
    {
        /// <summary>
        /// Runs the specified task.
        /// </summary>
        /// <param name="logProvider">The log provider.</param>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        public static Task AsyncRun(Action task)
        {
            return AsyncRun(task, TaskCreationOptions.None);
        }

        /// <summary>
        /// Runs the specified task.
        /// </summary>
        /// <param name="logProvider">The log provider.</param>
        /// <param name="task">The task.</param>
        /// <param name="taskOption">The task option.</param>
        /// <returns></returns>
        public static Task AsyncRun(Action task, TaskCreationOptions taskOption)
        {
            return AsyncRun(task, taskOption, null);
        }

        /// <summary>
        /// Runs the specified task.
        /// </summary>
        /// <param name="logProvider">The log provider.</param>
        /// <param name="task">The task.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <returns></returns>
        public static Task AsyncRun(Action task, Action<Exception> exceptionHandler)
        {
            return AsyncRun(task, TaskCreationOptions.None, exceptionHandler);
        }

        /// <summary>
        /// Runs the specified task.
        /// </summary>
        /// <param name="logProvider">The log provider.</param>
        /// <param name="task">The task.</param>
        /// <param name="taskOption">The task option.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <returns></returns>
        public static Task AsyncRun(Action task, TaskCreationOptions taskOption, Action<Exception> exceptionHandler)
        {
            return Task.Factory.StartNew(task, taskOption).ContinueWith(t =>
            {
                if (exceptionHandler != null)
                    exceptionHandler(t.Exception);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        /// <summary>
        /// Runs the specified task.
        /// </summary>
        /// <param name="logProvider">The log provider.</param>
        /// <param name="task">The task.</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public static Task AsyncRun(Action<object> task, object state)
        {
            return AsyncRun(task, state, TaskCreationOptions.None);
        }

        /// <summary>
        /// Runs the specified task.
        /// </summary>
        /// <param name="logProvider">The log provider.</param>
        /// <param name="task">The task.</param>
        /// <param name="state">The state.</param>
        /// <param name="taskOption">The task option.</param>
        /// <returns></returns>
        public static Task AsyncRun( Action<object> task, object state, TaskCreationOptions taskOption)
        {
            return AsyncRun(task, state, taskOption, null);
        }

        /// <summary>
        /// Runs the specified task.
        /// </summary>
        /// <param name="logProvider">The log provider.</param>
        /// <param name="task">The task.</param>
        /// <param name="state">The state.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <returns></returns>
        public static Task AsyncRun(Action<object> task, object state, Action<Exception> exceptionHandler)
        {
            return AsyncRun(task, state, TaskCreationOptions.None, exceptionHandler);
        }

        /// <summary>
        /// Runs the specified task.
        /// </summary>
        /// <param name="logProvider">The log provider.</param>
        /// <param name="task">The task.</param>
        /// <param name="state">The state.</param>
        /// <param name="taskOption">The task option.</param>
        /// <param name="exceptionHandler">The exception handler.</param>
        /// <returns></returns>
        public static Task AsyncRun(Action<object> task, object state, TaskCreationOptions taskOption, Action<Exception> exceptionHandler)
        {
            return Task.Factory.StartNew(task, state, taskOption).ContinueWith(t =>
            {
                if (exceptionHandler != null)
                    exceptionHandler(t.Exception);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
