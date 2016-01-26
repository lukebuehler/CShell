using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CShell.Modules.Repl.Controls
{
    public class CommandQueue<T>
    {
        private readonly int _limit;
        readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

        public int Count
        {
            get { return _queue.Count; }
        }

        public T this[int i]
        {
            get { return _queue.ToArray()[i]; }
        }

        public T[] ToArray()
        {
            return _queue.ToArray();
        }

        public CommandQueue(int limit)
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