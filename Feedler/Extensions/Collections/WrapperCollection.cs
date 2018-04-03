using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Feedler.Extensions.Collections
{
    public sealed class WrapperCollection<TSource, TWrapper>: ICollection<TWrapper>
    {
        private readonly ICollection<TSource> _source;
        private readonly Func<TSource, TWrapper> _wrap;
        private readonly Func<TWrapper, TSource> _unwrap;

        public WrapperCollection(ICollection<TSource> source,
            Func<TSource, TWrapper> wrapFunc, Func<TWrapper, TSource> unwrapFunc)
        {
            _source = source;
            _wrap = wrapFunc;
            _unwrap = unwrapFunc;
        }

        public IEnumerator<TWrapper> GetEnumerator() => _source.Select(_wrap).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TWrapper item) => _source.Add(_unwrap(item));

        public void Clear() => _source.Clear();

        public bool Contains(TWrapper item) => _source.Contains(_unwrap(item));

        public void CopyTo(TWrapper[] array, int arrayIndex) => throw new NotImplementedException();

        public bool Remove(TWrapper item) => _source.Remove(_unwrap(item));

        public int Count => _source.Count;

        public bool IsReadOnly => _source.IsReadOnly;
    }

}