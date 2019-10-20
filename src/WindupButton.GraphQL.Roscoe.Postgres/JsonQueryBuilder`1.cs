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
using WindupButton.GraphQL.Data;
using WindupButton.GraphQL.Schema;
using WindupButton.Roscoe;
using WindupButton.Roscoe.Expressions;
using WindupButton.Roscoe.Postgres.Commands;

namespace WindupButton.GraphQL.Roscoe.Postgres
{
    public class JsonQueryBuilder
    {
        private readonly JsonQueryBuilderAdapter adapter;
        private readonly Func<object> commandResultAccessor;

        private static int counter;

        internal JsonQueryBuilder(JsonQueryBuilderAdapter adapter, Func<object> commandResultAccessor)
        {
            Check.IsNotNull(adapter, nameof(adapter));
            Check.IsNotNull(commandResultAccessor, nameof(commandResultAccessor));

            this.adapter = adapter;
            this.commandResultAccessor = commandResultAccessor;
        }

        public IValueAccessor<GraphQLNotNull<TScalar>> Select<TScalar, T>(IDbFragment<T> value)
            where TScalar : IGraphQLType<T>, new()
        {
            var name = GetNextName();
            adapter.Select(name, value);

            return new ValueAccessor<GraphQLNotNull<TScalar>>(new GraphQLNotNull<TScalar>(new TScalar()), name, typeof(T), commandResultAccessor);
        }

        public IValueAccessor<GraphQLNotNull<TScalar>> Select<TScalar, T1, T>(
            IDbFragment<T1> value1,
            Func<T1, T> resultTransformer)
                where TScalar : IGraphQLType<T>, new()
        {
            var names = new[]
            {
                SelectName(value1),
            };

            return new ValueAccessor<GraphQLNotNull<TScalar>>(
                new GraphQLNotNull<TScalar>(new TScalar()),
                names,
                x => resultTransformer((T1)x[0]),
                commandResultAccessor);
        }

        public IValueAccessor<GraphQLNotNull<TScalar>> Select<TScalar, T1, T2, T>(
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            Func<T1, T2, T> resultTransformer)
                where TScalar : IGraphQLType<T>, new()
        {
            var names = new[]
            {
                SelectName(value1),
                SelectName(value2),
            };

            return new ValueAccessor<GraphQLNotNull<TScalar>>(
                new GraphQLNotNull<TScalar>(new TScalar()),
                names,
                x => resultTransformer((T1)x[0], (T2)x[1]),
                commandResultAccessor);
        }

        public IValueAccessor<GraphQLNotNull<TScalar>> Select<TScalar, T1, T2, T3, T>(
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            Func<T1, T2, T3, T> resultTransformer)
                where TScalar : IGraphQLType<T>, new()
        {
            var names = new[]
            {
                SelectName(value1),
                SelectName(value2),
                SelectName(value3),
            };

            return new ValueAccessor<GraphQLNotNull<TScalar>>(
                new GraphQLNotNull<TScalar>(new TScalar()),
                names,
                x => resultTransformer((T1)x[0], (T2)x[1], (T3)x[2]),
                commandResultAccessor);
        }

        public IValueAccessor<GraphQLNotNull<TScalar>> Select<TScalar, T1, T2, T3, T4, T>(
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            Func<T1, T2, T3, T4, T> resultTransformer)
                where TScalar : IGraphQLType<T>, new()
        {
            var names = new[]
            {
                SelectName(value1),
                SelectName(value2),
                SelectName(value3),
                SelectName(value4),
            };

            return new ValueAccessor<GraphQLNotNull<TScalar>>(
                new GraphQLNotNull<TScalar>(new TScalar()),
                names,
                x => resultTransformer((T1)x[0], (T2)x[1], (T3)x[2], (T4)x[3]),
                commandResultAccessor);
        }

        public IValueAccessor<GraphQLNotNull<TScalar>> Select<TScalar, T1, T2, T3, T4, T5, T>(
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            Func<T1, T2, T3, T4, T5, T> resultTransformer)
                where TScalar : IGraphQLType<T>, new()
        {
            var names = new[]
            {
                SelectName(value1),
                SelectName(value2),
                SelectName(value3),
                SelectName(value4),
                SelectName(value5),
            };

            return new ValueAccessor<GraphQLNotNull<TScalar>>(
                new GraphQLNotNull<TScalar>(new TScalar()),
                names,
                x => resultTransformer((T1)x[0], (T2)x[1], (T3)x[2], (T4)x[3], (T5)x[4]),
                commandResultAccessor);
        }

        public IValueAccessor<GraphQLNotNull<TScalar>> Select<TScalar, T1, T2, T3, T4, T5, T6, T>(
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            IDbFragment<T6> value6,
            Func<T1, T2, T3, T4, T5, T6, T> resultTransformer)
                where TScalar : IGraphQLType<T>, new()
        {
            var names = new[]
            {
                SelectName(value1),
                SelectName(value2),
                SelectName(value3),
                SelectName(value4),
                SelectName(value5),
                SelectName(value6),
            };

            return new ValueAccessor<GraphQLNotNull<TScalar>>(
                new GraphQLNotNull<TScalar>(new TScalar()),
                names,
                x => resultTransformer((T1)x[0], (T2)x[1], (T3)x[2], (T4)x[3], (T5)x[4], (T6)x[5]),
                commandResultAccessor);
        }

        public IValueAccessor<GraphQLNotNull<TScalar>> Select<TScalar, T1, T2, T3, T4, T5, T6, T7, T>(
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            IDbFragment<T6> value6,
            IDbFragment<T7> value7,
            Func<T1, T2, T3, T4, T5, T6, T7, T> resultTransformer)
                where TScalar : IGraphQLType<T>, new()
        {
            var names = new[]
            {
                SelectName(value1),
                SelectName(value2),
                SelectName(value3),
                SelectName(value4),
                SelectName(value5),
                SelectName(value6),
                SelectName(value7),
            };

            return new ValueAccessor<GraphQLNotNull<TScalar>>(
                new GraphQLNotNull<TScalar>(new TScalar()),
                names,
                x => resultTransformer((T1)x[0], (T2)x[1], (T3)x[2], (T4)x[3], (T5)x[4], (T6)x[5], (T7)x[6]),
                commandResultAccessor);
        }

        // ----

        public IValueAccessor<TScalar> SelectNullable<TScalar, T>(IDbFragment<T> value)
            where TScalar : IGraphQLType<T>, new()
        {
            var name = GetNextName();
            adapter.Select(name, value);

            return new ValueAccessor<TScalar>(new TScalar(), name, typeof(T), commandResultAccessor);
        }

        public IValueAccessor<TScalar> SelectNullable<TScalar, T1, T>(
            IDbFragment<T1> value1,
            Func<T1, T> resultTransformer)
                where TScalar : IGraphQLType<T>, new()
        {
            var names = new[]
            {
                SelectName(value1),
            };

            return new ValueAccessor<TScalar>(
                new TScalar(),
                names,
                x => resultTransformer((T1)x[0]),
                commandResultAccessor);
        }

        public IValueAccessor<TScalar> SelectNullable<TScalar, T1, T2, T>(
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            Func<T1, T2, T> resultTransformer)
                where TScalar : IGraphQLType<T>, new()
        {
            var names = new[]
            {
                SelectName(value1),
                SelectName(value2),
            };

            return new ValueAccessor<TScalar>(
                new TScalar(),
                names,
                x => resultTransformer((T1)x[0], (T2)x[1]),
                commandResultAccessor);
        }

        public IValueAccessor<TScalar> SelectNullable<TScalar, T1, T2, T3, T>(
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            Func<T1, T2, T3, T> resultTransformer)
                where TScalar : IGraphQLType<T>, new()
        {
            var names = new[]
            {
                SelectName(value1),
                SelectName(value2),
                SelectName(value3),
            };

            return new ValueAccessor<TScalar>(
                new TScalar(),
                names,
                x => resultTransformer((T1)x[0], (T2)x[1], (T3)x[2]),
                commandResultAccessor);
        }

        public IValueAccessor<TScalar> SelectNullable<TScalar, T1, T2, T3, T4, T>(
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            Func<T1, T2, T3, T4, T> resultTransformer)
                where TScalar : IGraphQLType<T>, new()
        {
            var names = new[]
            {
                SelectName(value1),
                SelectName(value2),
                SelectName(value3),
                SelectName(value4),
            };

            return new ValueAccessor<TScalar>(
                new TScalar(),
                names,
                x => resultTransformer((T1)x[0], (T2)x[1], (T3)x[2], (T4)x[3]),
                commandResultAccessor);
        }

        public IValueAccessor<TScalar> SelectNullable<TScalar, T1, T2, T3, T4, T5, T>(
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            Func<T1, T2, T3, T4, T5, T> resultTransformer)
                where TScalar : IGraphQLType<T>, new()
        {
            var names = new[]
            {
                SelectName(value1),
                SelectName(value2),
                SelectName(value3),
                SelectName(value4),
                SelectName(value5),
            };

            return new ValueAccessor<TScalar>(
                new TScalar(),
                names,
                x => resultTransformer((T1)x[0], (T2)x[1], (T3)x[2], (T4)x[3], (T5)x[4]),
                commandResultAccessor);
        }

        public IValueAccessor<TScalar> SelectNullable<TScalar, T1, T2, T3, T4, T5, T6, T>(
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            IDbFragment<T6> value6,
            Func<T1, T2, T3, T4, T5, T6, T> resultTransformer)
                where TScalar : IGraphQLType<T>, new()
        {
            var names = new[]
            {
                SelectName(value1),
                SelectName(value2),
                SelectName(value3),
                SelectName(value4),
                SelectName(value5),
                SelectName(value6),
            };

            return new ValueAccessor<TScalar>(
                new TScalar(),
                names,
                x => resultTransformer((T1)x[0], (T2)x[1], (T3)x[2], (T4)x[3], (T5)x[4], (T6)x[5]),
                commandResultAccessor);
        }

        public IValueAccessor<TScalar> SelectNullable<TScalar, T1, T2, T3, T4, T5, T6, T7, T>(
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            IDbFragment<T6> value6,
            IDbFragment<T7> value7,
            Func<T1, T2, T3, T4, T5, T6, T7, T> resultTransformer)
                where TScalar : IGraphQLType<T>, new()
        {
            var names = new[]
            {
                SelectName(value1),
                SelectName(value2),
                SelectName(value3),
                SelectName(value4),
                SelectName(value5),
                SelectName(value6),
                SelectName(value7),
            };

            return new ValueAccessor<TScalar>(
                new TScalar(),
                names,
                x => resultTransformer((T1)x[0], (T2)x[1], (T3)x[2], (T4)x[3], (T5)x[4], (T6)x[5], (T7)x[6]),
                commandResultAccessor);
        }

        // ----

        public IValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<TScalar>>>> SelectScalarList<TScalar, T>(DbArray<T> expression)
            where TScalar : IGraphQLType<T>, new()
        {
            var name = GetNextName();

            adapter.Select(name, expression);

            return new ValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<TScalar>>>>(
                new GraphQLNotNull<GraphQLList<GraphQLNotNull<TScalar>>>(new GraphQLList<GraphQLNotNull<TScalar>>(new GraphQLNotNull<TScalar>(new TScalar()))),
                name,
                typeof(TScalar),
                commandResultAccessor);
        }

        public IValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<TScalar>>>> SelectScalarList<TScalar, T>(
            IWrapper<PostgresQueryCommand> childQuery,
            IDbFragment<T> expression)
                where TScalar : IGraphQLType<T>, new()
        {
            var name = GetNextName();

            adapter.Select(name, childQuery, expression);

            return new ValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<TScalar>>>>(
                new GraphQLNotNull<GraphQLList<GraphQLNotNull<TScalar>>>(new GraphQLList<GraphQLNotNull<TScalar>>(new GraphQLNotNull<TScalar>(new TScalar()))),
                name,
                typeof(TScalar),
                commandResultAccessor);
        }

        public IValueAccessor<GraphQLList<GraphQLNotNull<TScalar>>> SelectNullableScalarList<TScalar, T>(
            IWrapper<PostgresQueryCommand> childQuery,
            IDbFragment<T> expression)
                where TScalar : IGraphQLType<T>, new()
        {
            var name = GetNextName();

            adapter.Select(name, childQuery, expression);

            return new ValueAccessor<GraphQLList<GraphQLNotNull<TScalar>>>(
                new GraphQLList<GraphQLNotNull<TScalar>>(new GraphQLNotNull<TScalar>(new TScalar())),
                name,
                typeof(TScalar),
                commandResultAccessor);
        }

        //    return new ValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<TScalar>>>>(
        //        new GraphQLNotNull<GraphQLList<GraphQLNotNull<TScalar>>>(new GraphQLList<GraphQLNotNull<TScalar>>(new GraphQLNotNull<TScalar>(new TScalar()))),
        //        name,
        //        null,
        //        commandResultAccessor);
        //}

        // Object selection

        public IValueAccessor<GraphQLNotNull<T>> Select<T>(
            IWrapper<PostgresQueryCommand> childQuery,
            Func<JsonQueryBuilder, T> factory)
                where T : GraphQLObject
        {
            return Select(childQuery, x => new GraphQLNotNull<T>(factory(x)), true);
        }

        //public IValueAccessor<GraphQLNotNull<T>> Select<TQuery, T>(
        //    IWrapper<PostgresQueryCommand<TQuery>> childQuery,
        //    Func<IValueAccessor<TQuery>, T> factory)
        //        where T : GraphQLObject
        //{
        //    //return Select(childQuery, x => new GraphQLNotNull<T>(factory(x)), true);
        //    var name = GetNextName();

        //    var result = factory(new JsonQueryBuilder(
        //        adapter.Select(childQuery, name, true),
        //        commandResultAccessor));

        //    return new ValueAccessor<GraphQLNotNull<T>>(new GraphQLNotNull<T>(result), name, null, commandResultAccessor);
        //}

        public IValueAccessor<T> SelectNullable<T>(
            IWrapper<PostgresQueryCommand> childQuery,
            Func<JsonQueryBuilder, T> factory)
                where T : GraphQLObject
        {
            return Select(childQuery, factory, true);
        }

        // ----

        public IValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<T>>>> SelectList<T>(
            IWrapper<PostgresQueryCommand> childQuery,
            Func<JsonQueryBuilder, T> factory)
                where T : GraphQLObject
        {
            return Select(
                childQuery,
                x => new GraphQLNotNull<GraphQLList<GraphQLNotNull<T>>>(new GraphQLList<GraphQLNotNull<T>>(new GraphQLNotNull<T>(factory(x)))),
                false);
        }

        public IValueAccessor<GraphQLNotNull<GraphQLList<T>>> SelectListOfNullable<T>(
            IWrapper<PostgresQueryCommand> childQuery,
            Func<JsonQueryBuilder, T> factory)
                where T : GraphQLObject
        {
            return Select(
                childQuery,
                x => new GraphQLNotNull<GraphQLList<T>>(new GraphQLList<T>(factory(x))),
                false);
        }

        public IValueAccessor<GraphQLList<GraphQLNotNull<T>>> SelectNullableList<T>(
            IWrapper<PostgresQueryCommand> childQuery,
            Func<JsonQueryBuilder, T> factory)
                where T : GraphQLObject
        {
            return Select(
                childQuery,
                x => new GraphQLList<GraphQLNotNull<T>>(new GraphQLNotNull<T>(factory(x))),
                false);
        }

        public IValueAccessor<GraphQLList<T>> SelectNullableListOfNullable<T>(
            IWrapper<PostgresQueryCommand> childQuery,
            Func<JsonQueryBuilder, T> factory)
                where T : GraphQLObject
        {
            return Select(
                childQuery,
                x => new GraphQLList<T>(factory(x)),
                false);
        }

        // ---

        public IValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<T>>>> ExpandList<T, T1>(
            IDbFragment<T1> expression1,
            Func<T1, object> valueTransformer,
            Func<T> factory)
                where T : GraphQLObject
        {
            Check.IsNotNull(expression1, nameof(expression1));
            Check.IsNotNull(valueTransformer, nameof(valueTransformer));
            Check.IsNotNull(factory, nameof(factory));

            var names = new[]
            {
                SelectName(expression1, false),
            };

            return ValueAccessor.Create(
                new GraphQLNotNull<GraphQLList<GraphQLNotNull<T>>>(new GraphQLList<GraphQLNotNull<T>>(new GraphQLNotNull<T>(factory()))),
                names,
                x => valueTransformer((T1)x[0]),
                commandResultAccessor);
        }

        // ---

        private IValueAccessor<T> Select<T>(
            IWrapper<PostgresQueryCommand> childQuery,
            Func<JsonQueryBuilder, T> factory,
            bool isSingular)
                where T : IGraphQLType
        {
            var name = GetNextName();

            var result = factory(new JsonQueryBuilder(
                adapter.Select(childQuery, name, isSingular),
                commandResultAccessor));

            return new ValueAccessor<T>(result, name, null, commandResultAccessor);
        }

        private (string, Type) SelectName<T>(IDbFragment<T> expression, bool includeType = true)
        {
            var name = GetNextName();
            adapter.Select(name, expression);

            return (name, includeType ? typeof(T) : null);
        }

        private string GetNextName()
        {
            return $"field{counter++}";
        }
    }
}
