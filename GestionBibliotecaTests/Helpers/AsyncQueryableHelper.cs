using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using System;

namespace GestionBibliotecaTests.Helpers
{
    public static class AsyncQueryableHelper
    {
        public static IQueryable<T> BuildMockAsyncQueryable<T>(this IEnumerable<T> source)
        {
            return new TestAsyncEnumerable<T>(source);
        }

        private class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
            public TestAsyncEnumerable(Expression expression) : base(expression) { }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }

            IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
        }

        private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;
            public TestAsyncEnumerator(IEnumerator<T> inner) { _inner = inner; }

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return ValueTask.CompletedTask;
            }

            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());

            public T Current => _inner.Current;
        }

        private class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;
            internal TestAsyncQueryProvider(IQueryProvider inner) { _inner = inner; }

            public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<TEntity>(expression);

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(expression);

            public object? Execute(Expression expression) => _inner.Execute(expression);

            public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);

            public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
            {
                return new TestAsyncEnumerable<TResult>(expression);
            }

            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                return Execute<TResult>(expression);
            }
        }
    }
}
