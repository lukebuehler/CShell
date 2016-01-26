using System.Collections.Generic;
using System.Linq;
using CShell.Modules.Repl.Controls;
using NUnit.Framework;

namespace CShell.Tests
{
    public class CommandQueueTest
    {
        [Test]
        public void AddItemToQueue_Limit1_ExpectAdded()
        {
           var queue = new CommandQueue<string>(1);
           const string expected = "Test string"; 

           queue.Add(expected);

           List<string> actual = queue.Contents().ToList();

           Assert.AreEqual(expected, actual[0]);
        }

        [Test]
        public void AddMultipleItemsToQueue_Limit1_ExpectLimitAdhered()
        {
            const int expectedQueueLength = 1;
           
            var queue = new CommandQueue<string>(expectedQueueLength);

            queue.Add("Test String 1");
            queue.Add("Test String 2");

            int actualQueueLength = queue.Contents().Count();

            Assert.AreEqual(expectedQueueLength,actualQueueLength);
        }

        [Test]
        public void AddMultipleItemsToQueue_Limit1_ExpectLastElementKept()
        {
            const string expectedElementKept = "Test String 2";
            
            var queue = new CommandQueue<string>(1);

            queue.Add("Test String 1");
            queue.Add("Test String 2");

            string actualElementKept = queue.Contents().Single();

            Assert.AreEqual(expectedElementKept, actualElementKept);
        }

        [Test]
        //[TestCase("Test String 1", -1] We don't test String 1 as it should've been removed from queue
        [TestCase("Test String 2", 0)]
        [TestCase("Test String 3", 1)]
        [TestCase("Test String 4", 2)]
        public void AddMultipleItemsToQueue_Limit3_ExpectLastElementKept(string actual, int expectedQueuePosition)
        {
            var queue = new CommandQueue<string>(3);

            queue.Add("Test String 1");
            queue.Add("Test String 2");
            queue.Add("Test String 3");
            queue.Add("Test String 4");

            string[] actualElementKept = queue.Contents().ToArray();

            Assert.AreEqual(actual, actualElementKept[expectedQueuePosition]);
            
        }
    }
}
