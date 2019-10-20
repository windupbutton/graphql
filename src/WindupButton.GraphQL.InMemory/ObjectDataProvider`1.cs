// Copyright 2019 Windup Button
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WindupButton.GraphQL.Data;
using WindupButton.GraphQL.Schema;

namespace WindupButton.GraphQL.InMemory
{
    public sealed class ObjectDataProvider<TData> : IAsyncOperation
    {
        private readonly Func<CancellationToken, Task<object>> valueFactory;

        private object result;

        public ObjectDataProvider()
        {
            valueFactory = token => Task.FromResult<object>(null);
        }

        public ObjectDataProvider(Func<CancellationToken, Task<TData>> valueFactory)
        {
            Check.IsNotNull(valueFactory, nameof(valueFactory));

            this.valueFactory = async token => await valueFactory(token);
        }

        public ObjectDataProvider(Func<CancellationToken, Task<IEnumerable<TData>>> valueFactory)
        {
            Check.IsNotNull(valueFactory, nameof(valueFactory));

            this.valueFactory = async token => await valueFactory(token);
        }

        private ObjectDataProvider(Func<CancellationToken, Task<object>> valueFactory)
        {
            Check.IsNotNull(valueFactory, nameof(valueFactory));

            this.valueFactory = valueFactory;
        }

        public async Task ExecuteAsync(CancellationToken token)
        {
            result = await valueFactory(token);
        }

        public ObjectDataProvider<T> CreateChildProvider<T>(Func<TData, T> selector)
        {
            Check.IsNotNull(selector, nameof(selector));

            return new ObjectDataProvider<T>(async token =>
            {
                var value = await valueFactory(token);

                if (value is IEnumerable<TData> enumerable)
                {
                    return enumerable.Select(selector).ToList();
                }

                return selector((TData)value);
            });
        }

        public ObjectDataProvider<T> CreateChildProviderForCollection<T>(Func<TData, IEnumerable<T>> selector)
        {
            Check.IsNotNull(selector, nameof(selector));

            return new ObjectDataProvider<T>(async token =>
            {
                var value = await valueFactory(token);

                if (value is IEnumerable<TData> enumerable)
                {
                    // todo: is SelectMany the right approach here? Should we be throwing instead?

                    return enumerable.SelectMany(selector).ToList();
                }

                return selector((TData)value);
            });
        }

        public IValueAccessor<GraphQLNotNull<T>> Select<T>(Func<TData, object> selector)
            where T : IGraphQLType, new()
        {
            return new ValueAccessor<GraphQLNotNull<T>>(new GraphQLNotNull<T>(new T()), () => result, CreateSelector(selector));
        }

        public IValueAccessor<GraphQLNotNull<T>> Select<T>(Func<TData, object> selector, T graphQLObject)
            where T : GraphQLObject
        {
            return new ValueAccessor<GraphQLNotNull<T>>(new GraphQLNotNull<T>(graphQLObject), () => result, CreateSelector(selector));
        }

        public IValueAccessor<T> SelectNullable<T>(Func<TData, object> selector)
            where T : IGraphQLType, new()
        {
            return new ValueAccessor<T>(new T(), () => result, CreateSelector(selector));
        }

        public IValueAccessor<T> SelectNullable<T>(Func<TData, object> selector, T graphQLObject)
            where T : GraphQLObject
        {
            return new ValueAccessor<T>(graphQLObject, () => result, CreateSelector(selector));
        }

        public IValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<T>>>> SelectList<T>(Func<TData, object> selector)
            where T : IGraphQLType, new()
        {
            return new ValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<T>>>>(
                new GraphQLNotNull<GraphQLList<GraphQLNotNull<T>>>(new GraphQLList<GraphQLNotNull<T>>(new GraphQLNotNull<T>(new T()))),
                () => result,
                CreateSelector(selector));
        }

        public IValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<T>>>> SelectList<T>(Func<TData, object> selector, T graphQLObject)
            where T : GraphQLObject
        {
            return new ValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<T>>>>(
                new GraphQLNotNull<GraphQLList<GraphQLNotNull<T>>>(new GraphQLList<GraphQLNotNull<T>>(new GraphQLNotNull<T>(graphQLObject))),
                () => result,
                CreateSelector(selector));
        }

        public IValueAccessor<GraphQLList<GraphQLNotNull<T>>> SelectNullableList<T>(Func<TData, object> selector)
            where T : IGraphQLType, new()
        {
            return new ValueAccessor<GraphQLList<GraphQLNotNull<T>>>(
                new GraphQLList<GraphQLNotNull<T>>(new GraphQLNotNull<T>(new T())),
                () => result,
                CreateSelector(selector));
        }

        public IValueAccessor<GraphQLList<GraphQLNotNull<T>>> SelectNullableList<T>(Func<TData, object> selector, T graphQLObject)
            where T : GraphQLObject
        {
            return new ValueAccessor<GraphQLList<GraphQLNotNull<T>>>(
                new GraphQLList<GraphQLNotNull<T>>(new GraphQLNotNull<T>(graphQLObject)),
                () => result,
                CreateSelector(selector));
        }

        public IValueAccessor<GraphQLNotNull<GraphQLList<T>>> SelectListOfNullable<T>(Func<TData, object> selector)
            where T : IGraphQLType, new()
        {
            return new ValueAccessor<GraphQLNotNull<GraphQLList<T>>>(
                new GraphQLNotNull<GraphQLList<T>>(new GraphQLList<T>(new T())),
                () => result,
                CreateSelector(selector));
        }

        public IValueAccessor<GraphQLNotNull<GraphQLList<T>>> SelectListOfNullable<T>(Func<TData, object> selector, T graphQLObject)
            where T : GraphQLObject
        {
            return new ValueAccessor<GraphQLNotNull<GraphQLList<T>>>(
                new GraphQLNotNull<GraphQLList<T>>(new GraphQLList<T>(graphQLObject)),
                () => result,
                CreateSelector(selector));
        }

        public IValueAccessor<GraphQLList<T>> SelectNullableListOfNullable<T>(Func<TData, object> selector)
            where T : IGraphQLType, new()
        {
            return new ValueAccessor<GraphQLList<T>>(new GraphQLList<T>(new T()), () => result, CreateSelector(selector));
        }

        public IValueAccessor<GraphQLList<T>> SelectNullableListOfNullable<T>(Func<TData, object> selector, T graphQLObject)
            where T : GraphQLObject
        {
            return new ValueAccessor<GraphQLList<T>>(new GraphQLList<T>(graphQLObject), () => result, CreateSelector(selector));
        }

        private Func<object, object> CreateSelector(Func<TData, object> selector)
        {
            Check.IsNotNull(selector, nameof(selector));

            return x =>
            {
                return x is TData data
                    ? selector(data)
                    : x is IEnumerable enumerable
                    ? enumerable.Cast<TData>().Select(y => selector(y)).ToArray()
                    : throw new InvalidOperationException(); // todo: message
            };
        }
    }
}
