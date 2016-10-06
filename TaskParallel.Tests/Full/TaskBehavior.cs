﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace System.Tests
{
    [TestClass]
    public class TaskBehavior
    {
        const int Timeout = 150;
        const string ExceptionMessage = "Task Exception";
        const string TaskExecutionTimeout = "Task execution could not be completed into expected time";

        [TestMethod]
        [Timeout(Timeout)]
        public void TaskContinueExceptionTest()
        {
            var task = Task.Factory
                .StartNew(() => { throw new Exception(ExceptionMessage); })
                .ContinueWith(t =>
                {
                    Assert.IsTrue(t.IsFaulted);
                });
            task.Wait();
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void TaskWaitExceptionTest()
        {
            var task = Task.Factory.StartNew(() =>
            {
                Task.Delay(Timeout).Wait();
                throw new Exception(ExceptionMessage);
            });
            if (!task.Wait(Timeout * 2))
                Assert.Fail(TaskExecutionTimeout);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void TaskResultExceptionStarted()
        {
            var task = Task.Factory.StartNew<int>(() => { throw new Exception(ExceptionMessage); });
            Assert.AreEqual(0, task.Result);
        }

        [TestMethod]
        public void TaskResultCreatedTest()
        {
            var outerTask = Task.Run(() =>
            {
                var task = new Task<int>(() => 15);
                return task.Result;
            });
            Assert.IsFalse(outerTask.Wait(Timeout));
        }

        [TestMethod]
        public void TaskAwaitCreatedTest()
        {
            //int counter = 0;
            //var task = new Task(() => Interlocked.Increment(ref counter));
            //if (await Task.WhenAny(task, Task.Delay(Timeout)) == task)
            //    Assert.Fail("Awaiting for task should not start it");

            //Assert.AreEqual(0, counter);
            //Assert.AreEqual(TaskStatus.Created, task.Status);
            Assert.Inconclusive("Test not implemented");
        }

        [TestMethod]
        public void TaskWaitCreatedTest()
        {
            int counter = 0;
            var task = new Task(() => Interlocked.Increment(ref counter));
            if (task.Wait(Timeout))
                Assert.Fail("Waiting for task should not start it");

            Assert.AreEqual(0, counter);
            Assert.AreEqual(TaskStatus.Created, task.Status);
        }

        [TestMethod]
        public void TaskWaitStartedTest()
        {
            int counter = 0;
            var task = new Task(() => Interlocked.Increment(ref counter));

            task.Start();
            //Assert.AreEqual(TaskStatus.WaitingToRun, task.Status);

            if (!task.Wait(Timeout))
                Assert.Fail(TaskExecutionTimeout);

            Assert.AreEqual(1, counter);
            Assert.AreEqual(TaskStatus.RanToCompletion, task.Status);
        }

        [TestMethod]
        public void TaskRunSyncExceptionTest()
        {
            var task = new Task(() => { throw new Exception(ExceptionMessage); });
            task.RunSynchronously();
        }

        [TestMethod]
        public void TaskContinueCreatedTest()
        {
            var task = new Task(() => { });
            Assert.IsFalse(task.ContinueWith(t => { }).Wait(Timeout));
        }
    }
}
