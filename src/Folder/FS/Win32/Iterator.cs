using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace IOFile
{

    //internal abstract class Iterator<TSource> : IEnumerable<TSource>, IEnumerable, IEnumerator<TSource>, IDisposable, IEnumerator
    //{
    //    // public Iterator();
    //    public TSource Current { get; }

    //    public abstract void Dispose();
    //    public abstract IEnumerator<TSource> GetEnumerator();
    //    public abstract bool MoveNext();
    //    protected abstract Iterator<TSource> Clone();
    //    protected virtual void Dispose(bool disposing);
    //}

    internal abstract class Iterator<TSource> : IEnumerable<TSource>, IEnumerator<TSource>, IEnumerator
    {
        private int _threadId;
        internal int state;
        internal TSource current;

        public Iterator()
        {
            _threadId = Thread.CurrentThread.ManagedThreadId;
        }

        public TSource Current
        {
            get { return current; }
        }

        protected abstract Iterator<TSource> Clone();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            current = default(TSource);
            state = -1;
        }

        public IEnumerator<TSource> GetEnumerator()
        {
            if (_threadId == Thread.CurrentThread.ManagedThreadId && state == 0)
            {
                state = 1;
                return this;
            }

            Iterator<TSource> duplicate = Clone();
            duplicate.state = 1;
            return duplicate;
        }

        public abstract bool MoveNext();

        object IEnumerator.Current
        {
            get { return Current; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void IEnumerator.Reset()
        {
            throw new NotSupportedException();
        }
    }
}
