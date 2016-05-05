using System;
using System.Collections;
using System.Collections.Generic;

namespace Folder
{
    public interface ILastError
    {
        Exception LastError { get; set; }
    }

    public interface IFirstRecord : IEnumerable
    {
        bool Any { get; }
    }

    public interface IFirstRecord<T> : IEnumerable<T>, IEnumerator<T>
    {
        T FirstRecord { get; }
    }
}
