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
using System.Collections.Generic;
using System.Linq;
using WindupButton.GraphQL.Data;
using WindupButton.GraphQL.Schema;
using WindupButton.Roscoe;
using WindupButton.Roscoe.Expressions;

namespace WindupButton.GraphQL.Roscoe.Postgres
{
    public class QueryBuilder<TWrapper>
       where TWrapper : IRoscoeCommand, IWrapper<SelectClause>
    {
        private readonly IWrapper<TWrapper> command;
        private readonly Func<object> commandResultAccessor;
        private readonly RowGroup rowGroup;

        private static int counter;

        internal QueryBuilder(IWrapper<TWrapper> command, Func<object> commandResultAccessor, RowGroup rowGroup)
        {
            Check.IsNotNull(command, nameof(command));
            Check.IsNotNull(commandResultAccessor, nameof(commandResultAccessor));
            Check.IsNotNull(rowGroup, nameof(rowGroup));

            this.command = command;
            this.commandResultAccessor = commandResultAccessor;
            this.rowGroup = rowGroup;
        }

        public Func<object, object> Cast<T1>(Func<T1, object> func)
        {
            return (t1) => func((T1)t1);
        }

        public Func<object, object, object> Cast<T1, T2>(Func<T1, T2, object> func)
        {
            return (t1, t2) => func((T1)t1, (T2)t2);
        }

        public Func<object, object, object, object> Cast<T1, T2, T3>(Func<T1, T2, T3, object> func)
        {
            return (t1, t2, t3) => func((T1)t1, (T2)t2, (T3)t3);
        }

        public Func<object, object, object, object, object> Cast<T1, T2, T3, T4>(Func<T1, T2, T3, T4, object> func)
        {
            return (t1, t2, t3, t4) => func((T1)t1, (T2)t2, (T3)t3, (T4)t4);
        }

        public Func<object, object, object, object, object, object> Cast<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, object> func)
        {
            return (t1, t2, t3, t4, t5) => func((T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5);
        }

        public Func<object, object, object, object, object, object, object> Cast<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, object> func)
        {
            return (t1, t2, t3, t4, t5, t6) => func((T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6);
        }

        public Func<object, object, object, object, object, object, object, object> Cast<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, object> func)
        {
            return (t1, t2, t3, t4, t5, t6, t7) => func((T1)t1, (T2)t2, (T3)t3, (T4)t4, (T5)t5, (T6)t6, (T7)t7);
        }

        public IValueAccessor<GraphQLNotNull<T>> Select<T>(Func<IWrapper<TWrapper>, ISelection<TWrapper>> selector)
            where T : IGraphQLType, new()
        {
            var selection = selector(command);

            selection.Alias = GetNextName();

            return new ValueAccessor<GraphQLNotNull<T>>(new GraphQLNotNull<T>(new T()), selection.Alias, null, commandResultAccessor);
        }

        public IValueAccessor<GraphQLNotNull<T>> Select<T>(
            Func<IWrapper<TWrapper>, (ISelection<TWrapper>, ISelection<TWrapper>)> selector,
            Func<object, object, object> resultTransformer)
                where T : IGraphQLType, new()
        {
            var selection = selector(command)
                .ToArray()
                .Select(x => x.Alias = GetNextName())
                .Select(x => (x, (Type)null))
                .ToList();

            return new ValueAccessor<GraphQLNotNull<T>>(
                new GraphQLNotNull<T>(new T()),
                selection,
                x => resultTransformer(x[0], x[1]),
                commandResultAccessor);
        }

        public IValueAccessor<GraphQLNotNull<T>> Select<T>(
            Func<IWrapper<TWrapper>, (ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>)> selector,
            Func<object, object, object, object> resultTransformer)
                where T : IGraphQLType, new()
        {
            var selection = selector(command)
                .ToArray()
                .Select(x => x.Alias = GetNextName())
                .Select(x => (x, (Type)null))
                .ToList();

            return new ValueAccessor<GraphQLNotNull<T>>(
                new GraphQLNotNull<T>(new T()),
                selection,
                x => resultTransformer(x[0], x[1], x[2]),
                commandResultAccessor);
        }

        public IValueAccessor<GraphQLNotNull<T>> Select<T>(
            Func<IWrapper<TWrapper>, (ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>)> selector,
            Func<object, object, object, object, object> resultTransformer)
                where T : IGraphQLType, new()
        {
            var selection = selector(command)
                .ToArray()
                .Select(x => x.Alias = GetNextName())
                .Select(x => (x, (Type)null))
                .ToList();

            return new ValueAccessor<GraphQLNotNull<T>>(
                new GraphQLNotNull<T>(new T()),
                selection,
                x => resultTransformer(x[0], x[1], x[2], x[3]),
                commandResultAccessor);
        }

        public IValueAccessor<GraphQLNotNull<T>> Select<T>(
            Func<IWrapper<TWrapper>, (ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>)> selector,
            Func<object, object, object, object, object, object> resultTransformer)
                where T : IGraphQLType, new()
        {
            var selection = selector(command)
                .ToArray()
                .Select(x => x.Alias = GetNextName())
                .Select(x => (x, (Type)null))
                .ToList();

            return new ValueAccessor<GraphQLNotNull<T>>(
                new GraphQLNotNull<T>(new T()),
                selection,
                x => resultTransformer(x[0], x[1], x[2], x[3], x[4]),
                commandResultAccessor);
        }

        public IValueAccessor<GraphQLNotNull<T>> Select<T>(
            Func<IWrapper<TWrapper>, (ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>)> selector,
            Func<object, object, object, object, object, object, object> resultTransformer)
                where T : IGraphQLType, new()
        {
            var selection = selector(command)
                .ToArray()
                .Select(x => x.Alias = GetNextName())
                .Select(x => (x, (Type)null))
                .ToList();

            return new ValueAccessor<GraphQLNotNull<T>>(
                new GraphQLNotNull<T>(new T()),
                selection,
                x => resultTransformer(x[0], x[1], x[2], x[3], x[4], x[5]),
                commandResultAccessor);
        }

        private IValueAccessor<GraphQLNotNull<T>> Select<T>(
           Func<IWrapper<TWrapper>, (ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>)> selector,
           Func<object, object, object, object, object, object, object, object> resultTransformer)
               where T : IGraphQLType, new()
        {
            var selection = selector(command)
                .ToArray()
                .Select(x => x.Alias = GetNextName())
                .Select(x => (x, (Type)null))
                .ToList();

            return new ValueAccessor<GraphQLNotNull<T>>(
                new GraphQLNotNull<T>(new T()),
                selection,
                x => resultTransformer(x[0], x[1], x[2], x[3], x[4], x[5], x[6]),
                commandResultAccessor);
        }

        public IValueAccessor<T> SelectNullable<T>(Func<IWrapper<TWrapper>, ISelection<TWrapper>> selector)
            where T : IGraphQLType, new()
        {
            var selection = selector(command);

            selection.Alias = GetNextName();

            return new ValueAccessor<T>(new T(), selection.Alias, null, commandResultAccessor);
        }

        public IValueAccessor<T> SelectNullable<T>(
            Func<IWrapper<TWrapper>, (ISelection<TWrapper>, ISelection<TWrapper>)> selector,
            Func<object, object, object> resultTransformer)
                where T : IGraphQLType, new()
        {
            var selection = selector(command)
                .ToArray()
                .Select(x => x.Alias = GetNextName())
                .Select(x => (x, (Type)null))
                .ToList();

            return new ValueAccessor<T>(
                new T(),
                selection,
                x => resultTransformer(x[0], x[1]),
                commandResultAccessor);
        }

        public IValueAccessor<T> SelectNullable<T>(
            Func<IWrapper<TWrapper>, (ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>)> selector,
            Func<object, object, object, object> resultTransformer)
                where T : IGraphQLType, new()
        {
            var selection = selector(command)
                .ToArray()
                .Select(x => x.Alias = GetNextName())
                .Select(x => (x, (Type)null))
                .ToList();

            return new ValueAccessor<T>(
                new T(),
                selection,
                x => resultTransformer(x[0], x[1], x[2]),
                commandResultAccessor);
        }

        public IValueAccessor<T> SelectNullable<T>(
            Func<IWrapper<TWrapper>, (ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>)> selector,
            Func<object, object, object, object, object> resultTransformer)
                where T : IGraphQLType, new()
        {
            var selection = selector(command)
                .ToArray()
                .Select(x => x.Alias = GetNextName())
                .Select(x => (x, (Type)null))
                .ToList();

            return new ValueAccessor<T>(
                new T(),
                selection,
                x => resultTransformer(x[0], x[1], x[2], x[3]),
                commandResultAccessor);
        }

        public IValueAccessor<T> SelectNullable<T>(
            Func<IWrapper<TWrapper>, (ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>)> selector,
            Func<object, object, object, object, object, object> resultTransformer)
                where T : IGraphQLType, new()
        {
            var selection = selector(command)
                .ToArray()
                .Select(x => x.Alias = GetNextName())
                .Select(x => (x, (Type)null))
                .ToList();

            return new ValueAccessor<T>(
                new T(),
                selection,
                x => resultTransformer(x[0], x[1], x[2], x[3], x[4]),
                commandResultAccessor);
        }

        public IValueAccessor<T> SelectNullable<T>(
            Func<IWrapper<TWrapper>, (ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>)> selector,
            Func<object, object, object, object, object, object, object> resultTransformer)
                where T : IGraphQLType, new()
        {
            var selection = selector(command)
                .ToArray()
                .Select(x => x.Alias = GetNextName())
                .Select(x => (x, (Type)null))
                .ToList();

            return new ValueAccessor<T>(
                new T(),
                selection,
                x => resultTransformer(x[0], x[1], x[2], x[3], x[4], x[5]),
                commandResultAccessor);
        }

        public IValueAccessor<T> SelectNullable<T>(
            Func<IWrapper<TWrapper>, (ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>, ISelection<TWrapper>)> selector,
            Func<object, object, object, object, object, object, object, object> resultTransformer)
                where T : IGraphQLType, new()
        {
            var selection = selector(command)
                .ToArray()
                .Select(x => x.Alias = GetNextName())
                .Select(x => (x, (Type)null))
                .ToList();

            return new ValueAccessor<T>(
                new T(),
                selection,
                x => resultTransformer(x[0], x[1], x[2], x[3], x[4], x[5], x[6]),
                commandResultAccessor);
        }

        public IValueAccessor<GraphQLNotNull<T>> Select<T>(
            Func<IWrapper<TWrapper>, IWrapper<TWrapper>> selector,
            Func<IWrapper<TWrapper>, IEnumerable<ISelection<TWrapper>>> keySelector,
            Func<QueryBuilder<TWrapper>, T> factory)
                where T : GraphQLObject
        {
            return Select(selector, keySelector, x => new GraphQLNotNull<T>(factory(x)), true);
        }

        public IValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<T>>>> SelectScalarList<T>(
            Func<IWrapper<TWrapper>, IWrapper<TWrapper>> selector,
            Func<IWrapper<TWrapper>, IEnumerable<ISelection<TWrapper>>> keySelector,
            Func<IWrapper<TWrapper>, ISelection<TWrapper>> factory)
                where T : IGraphQLType, new()
        {
            selector(command);

            var key = keySelector(command);
            var keyNames = key
                .Select(x => x.Alias = GetNextName())
                .ToList();
            var name = GetNextName();

            var childRowGroup = new RowGroup(name, keyNames, false);
            rowGroup.AddChild(childRowGroup);

            var selection = factory(command);

            selection.Alias = GetNextName();

            return ValueAccessor.Create(
                new GraphQLNotNull<GraphQLList<GraphQLNotNull<T>>>(new GraphQLList<GraphQLNotNull<T>>(new GraphQLNotNull<T>(new T()))),
                new[] { (name, (Type)null) },
                GetValue,
                commandResultAccessor);

            object GetValue(object[] values)
            {
                var value = values.FirstOrDefault();

                if (value is IEnumerable<object> list)
                {
                    value = list
                        .Select(x => x is IDictionary<string, object> dict ? dict[selection.Alias] : null)
                        .ToList();
                }

                return value;
            }
        }

        public IValueAccessor<T> SelectNullable<T>(
            Func<IWrapper<TWrapper>, IWrapper<TWrapper>> selector,
            Func<IWrapper<TWrapper>, IEnumerable<ISelection<TWrapper>>> keySelector,
            Func<QueryBuilder<TWrapper>, T> factory)
                where T : GraphQLObject
        {
            return Select(selector, keySelector, factory, true);
        }

        public IValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<T>>>> SelectList<T>(
            Func<IWrapper<TWrapper>, IWrapper<TWrapper>> selector,
            Func<IWrapper<TWrapper>, IEnumerable<ISelection<TWrapper>>> keySelector,
            Func<QueryBuilder<TWrapper>, T> factory)
                where T : GraphQLObject
        {
            return Select(
                selector,
                keySelector,
                x => new GraphQLNotNull<GraphQLList<GraphQLNotNull<T>>>(new GraphQLList<GraphQLNotNull<T>>(new GraphQLNotNull<T>(factory(x)))),
                false);
        }

        public IValueAccessor<GraphQLNotNull<GraphQLList<T>>> SelectListOfNullable<T>(
            Func<IWrapper<TWrapper>, IWrapper<TWrapper>> selector,
            Func<IWrapper<TWrapper>, IEnumerable<ISelection<TWrapper>>> keySelector,
            Func<QueryBuilder<TWrapper>, T> factory)
                where T : GraphQLObject
        {
            return Select(
                selector,
                keySelector,
                x => new GraphQLNotNull<GraphQLList<T>>(new GraphQLList<T>(factory(x))),
                false);
        }

        public IValueAccessor<GraphQLList<GraphQLNotNull<T>>> SelectNullableList<T>(
            Func<IWrapper<TWrapper>, IWrapper<TWrapper>> selector,
            Func<IWrapper<TWrapper>, IEnumerable<ISelection<TWrapper>>> keySelector,
            Func<QueryBuilder<TWrapper>, T> factory)
                where T : GraphQLObject
        {
            return Select(
                selector,
                keySelector,
                x => new GraphQLList<GraphQLNotNull<T>>(new GraphQLNotNull<T>(factory(x))),
                false);
        }

        public IValueAccessor<GraphQLList<T>> SelectNullableListOfNullable<T>(
            Func<IWrapper<TWrapper>, IWrapper<TWrapper>> selector,
            Func<IWrapper<TWrapper>, IEnumerable<ISelection<TWrapper>>> keySelector,
            Func<QueryBuilder<TWrapper>, T> factory)
                where T : GraphQLObject
        {
            return Select(
                selector,
                keySelector,
                x => new GraphQLList<T>(factory(x)),
                false);
        }

        // ---

        public IValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<T>>>> ExpandList<T>(
            Func<IWrapper<TWrapper>, ISelection<TWrapper>> selector,
            Func<object, object> valueTransformer,
            Func<T> factory)
                where T : GraphQLObject
        {
            Check.IsNotNull(selector, nameof(selector));
            Check.IsNotNull(valueTransformer, nameof(valueTransformer));
            Check.IsNotNull(factory, nameof(factory));

            var selection = selector(command);

            selection.Alias = GetNextName();

            return ValueAccessor.Create(
                new GraphQLNotNull<GraphQLList<GraphQLNotNull<T>>>(new GraphQLList<GraphQLNotNull<T>>(new GraphQLNotNull<T>(factory()))),
                new[] { (selection.Alias, (Type)null) },
                x => valueTransformer(x[0]),
                commandResultAccessor);
        }

        // ---

        private IValueAccessor<T> Select<T>(
            Func<IWrapper<TWrapper>, IWrapper<TWrapper>> selector,
            Func<IWrapper<TWrapper>, IEnumerable<ISelection<TWrapper>>> keySelector,
            Func<QueryBuilder<TWrapper>, T> factory,
            bool isSingular)
                where T : IGraphQLType
        {
            selector(command);

            var key = keySelector(command);
            var keyNames = key
                .Select(x => x.Alias = GetNextName())
                .ToList();
            var name = GetNextName();

            var childRowGroup = new RowGroup(name, keyNames, isSingular);
            rowGroup.AddChild(childRowGroup);

            var result = factory(new QueryBuilder<TWrapper>(
                command,
                commandResultAccessor,
                childRowGroup));

            return new ValueAccessor<T>(result, name, null, commandResultAccessor);
        }

        private string GetNextName()
        {
            return $"field{counter++}";
        }
    }
}
