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
        }

        public IEnumerable<T> Contents()
        {
            return _queue;
        }
    }
}
