using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NeoSharp.Core.TaskManagers
{
    public abstract class IntervalScheduler
    {
        private static readonly List<CancellationTokenSource> _cancellationTokens = new List<CancellationTokenSource>();

        public static async Task RunTask(TimeSpan interval, CancellationTokenSource cancellationToken, Func<Task> action)
        {
            CheckAndStoreCancellationToken(cancellationToken);
            
            while (!cancellationToken.IsCancellationRequested)
            {
                await action();
                await Task.Delay(interval, cancellationToken.Token);
            }
        }

        public static async Task RunTask(TimeSpan interval, CancellationTokenSource cancellationToken, int repetitions, Func<Task> action)
        {
            CheckAndStoreCancellationToken(cancellationToken);
            
            var countRepeat = 0;

            while (!cancellationToken.IsCancellationRequested && countRepeat < repetitions)
            {
                await action();
                await Task.Delay(interval, cancellationToken.Token);
                countRepeat++;
            }
        }

        public static async Task RunTask(TimeSpan interval, int repetitions, Func<Task> action)
        {
            await RunTask(interval, new CancellationTokenSource(), repetitions, action);
        }

        public static async void Run(TimeSpan interval, CancellationTokenSource cancellationToken, Func<Task> action)
        {
            await RunTask(interval, cancellationToken, action);
        }

        public static async void Run(TimeSpan interval, CancellationTokenSource cancellationToken, int repetitions,
            Func<Task> action)
        {
            await RunTask(interval, cancellationToken, repetitions, action);
        }

        public static async void Run(TimeSpan interval, int repetitions, Func<Task> action)
        {
            await RunTask(interval, new CancellationTokenSource(), repetitions, action);
        }

        public static void CancelAllTasks()
        {
            _cancellationTokens.ForEach(c => c.Cancel());
        }

        private static void CheckAndStoreCancellationToken(CancellationTokenSource cancellationToken)
        {
            if (cancellationToken == null)
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }
            
            if (!_cancellationTokens.Contains(cancellationToken))
            {
                _cancellationTokens.Add(cancellationToken);
            }
        } 
    }
}
