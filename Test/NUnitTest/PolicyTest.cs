using GYB.Common.Polly;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Tests
{
    public class PolicyTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void RetryExecTest()
        {
            try
            {
                PolicyHelper.RetryExec<DivideByZeroException>(() =>
                {
                    Console.WriteLine("func.........");
                    throw new DivideByZeroException();
                });
            }
            catch (Exception ex)
            {
                
            }
            Assert.Pass();
        }

        [Test]
        public async Task RetryExecAsyncTest()
        {
            try
            {
                await PolicyHelper.RetryExecAsync<DivideByZeroException>(async () =>
                {
                    Console.WriteLine("func.........");
                    throw new DivideByZeroException();
                });
            }
            catch (Exception ex)
            {

            }
            Assert.Pass();
        }
    }
}