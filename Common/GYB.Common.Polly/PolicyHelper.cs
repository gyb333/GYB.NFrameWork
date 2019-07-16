using Polly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GYB.Common.Polly
{
    public class PolicyHelper
    {
        

        public static async Task RetryExecAsync<TExcetion>(Func<Task> func, int RetryCount = 3, int Seconds = 3)
                where TExcetion : Exception
        {
            await Policy.Handle<TExcetion>()
                 //重试次数
                 .WaitAndRetryAsync(RetryCount
                  //第次间隔3s, count为重试的索引
                  , sleepDurationProvider: (count, context) =>
                 {
                     return TimeSpan.FromSeconds(Seconds);
                 }, onRetry: (exception, timespan, retryCount, context) =>
                {
                    //定义故障的处理方法
                    Console.WriteLine(exception.Message + timespan + retryCount + context);

                }).ExecuteAsync(func);
        }
        public static async Task RetryExecAsync<TExcetion>(Func<Task> func, IEnumerable<TimeSpan> sleepDurations)
            where TExcetion : Exception
        {
            await Policy.Handle<TExcetion>()
                 //重试次数
                 .WaitAndRetryAsync(sleepDurations, onRetry: (exception, timespan, retryCount, context) =>
                  {
                      //定义故障的处理方法
                      Console.WriteLine(exception.Message + timespan + retryCount + context);

                  }).ExecuteAsync(func);
        }

        public static void RetryExec<TExcetion>(Action action, int RetryCount = 3, int Seconds = 3)
            where TExcetion : Exception
        {
            //次数+时间间隔
            Policy.Handle<TExcetion>()
                 //重试次数
                 .WaitAndRetry(RetryCount
                 //第次间隔3s, count为重试的索引
                 , sleepDurationProvider: (count) =>
                 {
                     return TimeSpan.FromSeconds(Seconds);
                 }, onRetry: (exception, timespan, retryCount, context) =>
                 {
                     //定义故障的处理方法
                     Console.WriteLine(exception.Message + retryCount + context);
                 }).Execute(() =>
            {
                action();
            });
        }

        public static void RetryExec<TExcetion>(Action action, IEnumerable<TimeSpan> sleepDurations)
            where TExcetion : Exception
        {
            //次数+时间间隔
            Policy.Handle<TExcetion>().WaitAndRetry(sleepDurations, (exception, timeSpan, retryCount, context) =>
                 {
                     //定义故障的处理方法
                     Console.WriteLine(exception.Message + timeSpan + retryCount + context);
                 }).Execute(() =>
                 {
                     action();
                 });
        }
    }
}
