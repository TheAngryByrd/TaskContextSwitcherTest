using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TaskTester
{
   
    public class Class1
    {
        private const int rounds = 1000;
        private const TaskCreationOptions TaskOptions = TaskCreationOptions.LongRunning;
        private const int parallelThreads = 200;
        [Fact]
        public async Task TaskDelayWithFactory()
        {
            var sw = new Stopwatch();
            sw.Start();
            await UsingTaskFactory(CreateTaskSleepWork());
            sw.Stop();
            Console.WriteLine("Task.Delay took {0} milliseconds", sw.ElapsedMilliseconds);
        }

        [Fact]
        public async Task TaskMultipleDelayWithTaskFactory()
        {
            await Run(CreateTaskSleepWork(), UsingTaskFactory);
        }  
        [Fact]
        public async Task TaskMultipleDelayWithTaskRun()
        {
            await Run(CreateTaskSleepWork(), UsingTaskRun);
        }


        [Fact]
        public async Task ThreadsleepWithFactory()
        {
            var sw = new Stopwatch();
            sw.Start();
            await UsingTaskFactory(CreateThreadSleepWork());
            sw.Stop();
            Console.WriteLine("Took {0} milliseconds", sw.ElapsedMilliseconds);
        }  
        
        [Fact]
        public async Task MultipleThreadsleepWithTaskFactory()
        {           
            await Run(CreateThreadSleepWork(), UsingTaskFactory);
        }
        [Fact]
        public async Task MultipleThreadsleepWithTaskRun()
        {
            await Run(CreateThreadSleepWork(), UsingTaskRun);
        }


        private async Task Run(Action work, Func<Action, Task> runner, [CallerMemberName] string methodName = null)
        {
            var sw = new Stopwatch();
            sw.Start();
            var tasks = Enumerable.Range(0, parallelThreads).Select(_ => work).Select(runner);
            await Task.WhenAll(tasks);
            sw.Stop();
            Console.WriteLine("{0} Took {1} milliseconds", methodName, sw.ElapsedMilliseconds);
        }

        private async Task Run(Func<Task> work, Func<Func<Task>, Task> runner, [CallerMemberName] string methodName = null)
        {
            var sw = new Stopwatch();
            sw.Start();
            var tasks = Enumerable.Range(0, parallelThreads).Select(_ => work).Select(runner);
            await Task.WhenAll(tasks);
            sw.Stop();
            Console.WriteLine("{0} Took {1} milliseconds", methodName, sw.ElapsedMilliseconds);
        }

        private Task UsingTaskFactory(Action action)
        {
            return Task.Factory.StartNew(action, CancellationToken.None, TaskOptions, TaskScheduler.Default);
        } 
        private Task UsingTaskFactory(Func<Task> action)
        {
            return Task.Factory.StartNew(action, CancellationToken.None, TaskOptions, TaskScheduler.Default).Unwrap();
        }

        private Task UsingTaskRun(Action action)
        {
            return Task.Run(action);
        }

        private Task UsingTaskRun(Func<Task> action)
        {
            return Task.Run(action);
        }

        private Action CreateThreadSleepWork()
        {
            return () =>
            {
                for (int i = 0; i < rounds; i++)
                {
                    Thread.Sleep(1);
                }
            };
        }
        private Func<Task> CreateTaskSleepWork()
        {
            return async () =>
            {
                for (int i = 0; i < rounds; i++)
                {
                    await Task.Delay(1);
                }
            };
        }

    
    }
}
