//https://github.com/AIBrain/Librainian/blob/master/Collections/ThreadSafeList.cs

#region License & Information

// This notice must be kept visible in the source.
//
// This section of source code belongs to Rick@AIBrain.Org unless otherwise specified,
// or the original license has been overwritten by the automatic formatting of this code.
// Any unmodified sections of source code borrowed from other projects retain their original license and thanks goes to the Authors.
//
// Donations and Royalties can be paid via
// PayPal: paypal@aibrain.org
// bitcoin:1Mad8TxTqxKnMiHuZxArFvX8BuFEB9nqX2
// bitcoin:1NzEsF7eegeEWDr5Vr9sSSgtUC4aL6axJu
// litecoin:LeUxdU2w3o6pLZGVys5xpDZvvo8DUrjBp9
//
// Usage of the source code or compiled binaries is AS-IS.
// I am not responsible for Anything You Do.
//
// Contact me by email if you have any questions or helpful criticism.
//
// "Librainian/ThreadSafeList.cs" was last cleaned by Rick on 2014/08/13 at 10:37 PM

#endregion License & Information

using System.Threading;

namespace Librainian.Collections {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    ///     Just a simple thread safe collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <value>Version 1.7</value>
    /// <remarks>TODO replace locks with AsyncLocks</remarks>
    [CollectionDataContract]
    [DebuggerDisplay("Count={Count}")]
    public sealed class ThreadSafeList<T> : IList<T>
    {
        public int TimeoutForReads = 1;

        [DataMember]
        private readonly List<T> _items = new List<T>();

        private readonly ReaderWriterLockSlim _readerWriter;

        public ThreadSafeList(IEnumerable<T> items = null)
        {
            _readerWriter = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

            AddRange(items);
        }

        public long LongCount
        {
            get
            {
                if (!_readerWriter.TryEnterUpgradeableReadLock(TimeoutForReads))
                    return 0;

                try
                {
                    return _items.LongCount();
                }
                finally
                {
                    _readerWriter.ExitUpgradeableReadLock();
                }

                //lock (_items)
                //    return _items.LongCount();
            }
        }

        public int Count
        {
            get
            {
                if (!_readerWriter.TryEnterUpgradeableReadLock(TimeoutForReads))
                    return 0;

                try
                {
                    return _items.Count;
                }
                finally
                {
                    _readerWriter.ExitUpgradeableReadLock();
                }

                //lock (_items)
                //    return _items.Count;
            }
        }

        public Boolean IsReadOnly { get { return false; } }

        public T this[int index]
        {
            get
            {
                if (!_readerWriter.TryEnterUpgradeableReadLock(TimeoutForReads))
                    return default(T);

                try
                {
                    return _items[index];
                }
                finally
                {
                    _readerWriter.ExitUpgradeableReadLock();
                }

                //lock (_items)
                //    return _items[index];
            }

            set
            {
                if (!_readerWriter.TryEnterUpgradeableReadLock(TimeoutForReads))
                    return ;

                try
                {
                    _items[index] = value;
                }
                finally
                {
                    _readerWriter.ExitUpgradeableReadLock();
                }

                //lock (_items)
                //    _items[index] = value;
            }
        }

        public void Add(T item)
        {
            if (!_readerWriter.TryEnterUpgradeableReadLock(TimeoutForReads))
                return;

            try
            {
                _items.Add(item);
            }
            finally
            {
                _readerWriter.ExitUpgradeableReadLock();
            }

            //lock (_items)
            //    _items.Add(item);
        }

        public void Clear()
        {
            if (!_readerWriter.TryEnterUpgradeableReadLock(TimeoutForReads))
                return;

            try
            {
                _items.Clear();
            }
            finally
            {
                _readerWriter.ExitUpgradeableReadLock();
            }

            //lock (_items)
            //    _items.Clear();
        }

        public Boolean Contains(T item)
        {
            if (!_readerWriter.TryEnterUpgradeableReadLock(TimeoutForReads))
                return false;

            try
            {
                return _items.Contains(item);
            }
            finally
            {
                _readerWriter.ExitUpgradeableReadLock();
            }

            //lock (_items)
            //    return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (!_readerWriter.TryEnterUpgradeableReadLock(TimeoutForReads))
                return;

            try
            {
                _items.CopyTo(array, arrayIndex);
            }
            finally
            {
                _readerWriter.ExitUpgradeableReadLock();
            }

            //lock (_items)
            //    _items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Clone().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(T item)
        {
            if (!_readerWriter.TryEnterUpgradeableReadLock(TimeoutForReads))
                return -1;

            try
            {
                return _items.IndexOf(item);
            }
            finally
            {
                _readerWriter.ExitUpgradeableReadLock();
            }

            //lock (_items)
            //    return _items.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (!_readerWriter.TryEnterUpgradeableReadLock(TimeoutForReads))
                return;

            try
            {
                _items.Insert(index, item);            
            }
            finally
            {
                _readerWriter.ExitUpgradeableReadLock();
            }

            //lock (_items)
            //    _items.Insert(index, item);            
        }

        public Boolean Remove(T item)
        {
            if (!_readerWriter.TryEnterUpgradeableReadLock(TimeoutForReads))
                return false;

            try
            {
                return _items.Remove(item);         
            }
            finally
            {
                _readerWriter.ExitUpgradeableReadLock();
            }

            //lock (_items)
            //    return _items.Remove(item);         
        }

        public void RemoveAt(int index)
        {
            if (!_readerWriter.TryEnterUpgradeableReadLock(TimeoutForReads))
                return;

            try
            {
                _items.RemoveAt(index);
            }
            finally
            {
                _readerWriter.ExitUpgradeableReadLock();
            }

            //lock (_items)
            //    _items.RemoveAt(index);           
        }

        public Task AddAsync(T item)
        {
            return Task.Run(() =>
            {
                TryAdd(item);
            });
        }

        /// <summary>
        ///     Add in an enumerable of items.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="asParallel"></param>
        public void AddRange(IEnumerable<T> collection, Boolean asParallel = true)
        {
            if (collection == null)
                return;

            if (!_readerWriter.TryEnterUpgradeableReadLock(TimeoutForReads))
                return;

            try
            {
                _items.AddRange(asParallel ? collection.AsParallel() : collection);

            }
            finally
            {
                _readerWriter.ExitUpgradeableReadLock();
            }

            //lock (_items)
            //    _items.AddRange(asParallel ? collection.AsParallel() : collection);           
        }

        /// <summary>
        ///     Returns a new copy of all items in the <see cref="List{T}" />.
        /// </summary>
        /// <returns></returns>
        public List<T> Clone(Boolean asParallel = true)
        {
            if (!_readerWriter.TryEnterUpgradeableReadLock(TimeoutForReads))
                return null;

            try
            {
                return asParallel ? new List<T>(_items.AsParallel()) : new List<T>(_items);          
            }
            finally
            {
                _readerWriter.ExitUpgradeableReadLock();
            }

            //lock (_items)
            //    return asParallel ? new List<T>(_items.AsParallel()) : new List<T>(_items);          
        }


        public Boolean TryAdd(T item)
        {
            if (!_readerWriter.TryEnterUpgradeableReadLock(TimeoutForReads))
                return false;

            try
            {
                _items.Add(item);
                        return true;
            }
            catch (NullReferenceException) { }
            catch (ObjectDisposedException) { }
            catch (ArgumentNullException) { }
            catch (ArgumentOutOfRangeException) { }
            catch (ArgumentException) { }
            finally
            {
                _readerWriter.ExitUpgradeableReadLock();
            }
            return false;

            //try
            //{
            //    lock (_items)
            //    {
            //        _items.Add(item);
            //        return true;
            //    }
            //}
            //catch (NullReferenceException) { }
            //catch (ObjectDisposedException) { }
            //catch (ArgumentNullException) { }
            //catch (ArgumentOutOfRangeException) { }
            //catch (ArgumentException) { }
            //return false;
        }
    }
}