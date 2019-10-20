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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using WindupButton.GraphQL.Schema;

namespace WindupButton.GraphQL.Data
{
    public class GraphQLSchemaOperations
    {
        private const string selectObject = "selectObject";
        private const string selectNullObject = "selectNullObject";

        private const string selectList = "selectList";
        private const string selectNullList = "selectNullList";
        private const string selectListOfNull = "selectListOfNull";
        private const string selectNullListOfNull = "selectNullListOfNull";

        public static Dictionary<string, MethodInfo> Methods { get; }

        static GraphQLSchemaOperations()
        {
            Methods = typeof(GraphQLSchemaOperations)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.Name == nameof(Select))
                .ToDictionary(x => x.GetCustomAttribute<DescriptionAttribute>().Description, x => x);
        }

        [Description(selectObject)]
        public static T Select<TObject, T>(IValueAccessor<GraphQLNotNull<TObject>> value, DataReference dataReference, Func<TObject, DataReference, T> selector)
            where TObject : GraphQLObject
        {
            Check.IsNotNull(value, nameof(value));
            Check.IsNotNull(selector, nameof(selector));

            var childReference = new DataReference(value.GetValue(dataReference));

            return selector(value.GraphQLType.WrappedType, childReference);
        }

        [Description(selectNullObject)]
        public static T Select<TObject, T>(IValueAccessor<TObject> value, DataReference dataReference, Func<TObject, DataReference, T> selector)
            where TObject : GraphQLObject
        {
            Check.IsNotNull(value, nameof(value));
            Check.IsNotNull(selector, nameof(selector));

            var childReference = new DataReference(value.GetValue(dataReference));

            return selector(value.GraphQLType, childReference);
        }

        [Description(selectList)]
        public static IEnumerable<T> Select<TObject, T>(IValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<TObject>>>> value, DataReference dataReference, Func<TObject, DataReference, T> selector)
            where TObject : GraphQLObject
        {
            Check.IsNotNull(value, nameof(value));
            Check.IsNotNull(selector, nameof(selector));

            return Select(value, dataReference, selector, value.GraphQLType.WrappedType.WrappedType.WrappedType);
        }

        [Description(selectNullList)]
        public static IEnumerable<T> Select<TObject, T>(IValueAccessor<GraphQLList<GraphQLNotNull<TObject>>> value, DataReference dataReference, Func<TObject, DataReference, T> selector)
            where TObject : GraphQLObject
        {
            Check.IsNotNull(value, nameof(value));
            Check.IsNotNull(selector, nameof(selector));

            return Select(value, dataReference, selector, value.GraphQLType.WrappedType.WrappedType);
        }

        [Description(selectListOfNull)]
        public static IEnumerable<T> Select<TObject, T>(IValueAccessor<GraphQLNotNull<GraphQLList<TObject>>> value, DataReference dataReference, Func<TObject, DataReference, T> selector)
            where TObject : GraphQLObject
        {
            Check.IsNotNull(value, nameof(value));
            Check.IsNotNull(selector, nameof(selector));

            return Select(value, dataReference, selector, value.GraphQLType.WrappedType.WrappedType);
        }

        [Description(selectNullListOfNull)]
        public static IEnumerable<T> Select<TObject, T>(IValueAccessor<GraphQLList<TObject>> value, DataReference dataReference, Func<TObject, DataReference, T> selector)
            where TObject : GraphQLObject
        {
            Check.IsNotNull(value, nameof(value));
            Check.IsNotNull(selector, nameof(selector));

            return Select(value, dataReference, selector, value.GraphQLType.WrappedType);
        }

        private static IEnumerable<T> Select<TObject, T>(IValueAccessor value, DataReference dataReference, Func<TObject, DataReference, T> selector, TObject tObject)
            where TObject : GraphQLObject
        {
            if (value.GetValue(dataReference) is IEnumerable enumerable)
            {
                return enumerable
                    .OfType<object>()
                    .Select(x => selector(tObject, new DataReference(x)))
                    .ToList();
            }

            throw new Exception("not an enumerable");
        }
    }
}
