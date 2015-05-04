using System.Collections.Generic;

namespace MineLib.PCL.Graphics.Map
{
    public class ThreadSafeList<T>
    {
        private readonly object _criticalSection = new object();
        private readonly List<T> _list = new List<T>();

        public void Add(T value)
        {
            lock (_criticalSection) { _list.Add(value); }
        }

        public void AddRange(IEnumerable<T> value)
        {
            lock (_criticalSection) { _list.AddRange(value); }
        }

        public void Clear()
        {
            lock (_criticalSection) { _list.Clear(); }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_criticalSection) { _list.CopyTo(array, arrayIndex); }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_criticalSection) { return _list.GetEnumerator(); }
        }

        public int Count { get { lock (_criticalSection) { return _list.Count; } } }

        public T this[int index]
        {
            get
            {
                lock (_criticalSection)
                {
                    if (index > _list.Count || (index == 0 && _list.Count == 0))
                        return default(T);

                    return _list[index];
                }
            }
            set { lock (_criticalSection) { _list[index]= value; } }
        }
    }
}
