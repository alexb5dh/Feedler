using System;
using System.Collections.Generic;

namespace Feedler.Extensions.Collections
{
    public static class WrapperCollectionExtensions
    {
        public static WrapperCollection<TSource, TWrapper> Wrap<TSource, TWrapper>(
            this ICollection<TSource> source, Func<TSource, TWrapper> wrapFunc,
            Func<TWrapper, TSource> unwrapFunc) =>
            new WrapperCollection<TSource, TWrapper>(source, wrapFunc, unwrapFunc);
    }
}