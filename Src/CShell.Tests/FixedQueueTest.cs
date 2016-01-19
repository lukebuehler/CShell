using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace CShell.Tests
{
    public class FixedQueueTest
    {
        [Test]
        public void AddItemToQueue_Limit1_ExpectAdded()
        {
           var queue = new FixedQueue<string>(1);
           const string expected = "Test string"; 

           queue.Add(expected);

           List<string> actual = queue.Contents().ToList();

           Assert.AreEqual(expected, actual[0]);
        }

        [Test]
        public void AddMultipleItemsToQueue_Limit1_ExpectLimitAdhered()
        {
            const int expectedQueueLength = 1;
           
            var queue = new FixedQueue<string>(expectedQueueLength);

            queue.Add("Test String 1");
            queue.Add("Test String 2");

            int actualQueueLength = queue.Contents().Count();

            Assert.AreEqual(expectedQueueLength,actualQueueLength);
        }

        [Test]
        public void AddMultipleItemsToQueue_Limit1_ExpectLastElementKept()
        {
            const string expectedElementKept = "Test String 2";
            
            var queue = new FixedQueue<string>(1);

            queue.Add("Test String 1");
            queue.Add("Test String 2");

            string actualElementKept = queue.Contents().Single();

            Assert.AreEqual(expectedElementKept, actualElementKept);
        }
    } 

    public class FixedQueue<T>
    {
        private readonly int _limit;
        readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>(); 

        public FixedQueue(int limit)
        {
            _limit = limit;
        }

        public void Add(T item)
        {
            _queue.Enqueue(item);

            lock (this) //ensure threads don't step on each other
            {
                T overflow;
                while (_queue.Count > _limit && _queue.TryDequeue(out overflow)) ;
            }
            
        }

        public IEnumerable<T> Contents()
        {
            return _queue;
        }
    }
}
