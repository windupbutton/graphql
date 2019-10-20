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
using System.Linq;
using Newtonsoft.Json.Linq;
using WindupButton.GraphQL.Data;

namespace WindupButton.GraphQL.Schema
{
    public sealed class GraphQLListTypeAdapter : IGraphQLTypeAdapter
    {
        public CoercionResult CoerceInput(Type targetType, string name, object value)
        {
            Check.IsNotNull(targetType, nameof(targetType));

            if (!targetType.IsGenericType)
            {
                throw new InvalidOperationException(); // todo message
            }

            var elementType = targetType.GetGenericArguments()[0];
            var elementTypeAdapter = GraphQLTypeHelper.CreateTypeAdapter(elementType);

            var enumerableValue = value is IEnumerable enumerable
                ? enumerable.Cast<object>()
                : new object[] { value };

            var coercedValues = enumerableValue
                //.Select((x, i) => elementTypeAdapter.CoerceInput(elementType, $"{name}[{i}]", x))
                .Select((x, i) => elementTypeAdapter.CoerceInput(elementType, $"{name}[{i}]", x is JToken token ? token.ToObject<object>() : x))
                .ToList();

            var errors = coercedValues
                .SelectMany(x => x.Errors ?? Enumerable.Empty<CoercionError>())
                .ToList();

            var values = coercedValues
                .Select(x => x.Value)
                .ToList();

            return new CoercionResult(values, errors);
        }

        public CoercionResult CoerceResult(Type targetType, string name, object value)
        {
            Check.IsNotNull(targetType, nameof(targetType));

            if (!targetType.IsGenericType)
            {
                throw new InvalidOperationException(); // todo message
            }

            var elementType = targetType.GetGenericArguments()[0];

            if (value is IEnumerable enumerable)
            {
                var enumerableValue = enumerable.OfType<object>();

                var elementTypeAdapter = GraphQLTypeHelper.CreateTypeAdapter(elementType);

                var coercedValues = enumerableValue
                    //.Select((x, i) => elementTypeAdapter.CoerceResult(elementType, $"{name}[{i}]", x))
                    .Select((x, i) => elementTypeAdapter.CoerceResult(elementType, $"{name}[{i}]", x is JToken token ? token.ToObject<object>() : x))
                    .ToList();

                var errors = coercedValues
                    .SelectMany(x => x.Errors ?? Enumerable.Empty<CoercionError>())
                    .ToList();

                var values = coercedValues
                    .Select(x => x.Value)
                    .ToList();

                return new CoercionResult(values, errors);
            }

            return new CoercionResult(
                null,
                new[] { new CoercionError(name, GetTypeDescription(targetType), value) });
        }

        public GraphQLTypeDescription GetTypeDescription(Type targetType)
        {
            Check.IsNotNull(targetType, nameof(targetType));

            if (!targetType.IsGenericType)
            {
                throw new InvalidOperationException(); // todo message
            }

            var elementType = targetType.GetGenericArguments()[0];
            var elementTypeAdapter = GraphQLTypeHelper.CreateTypeAdapter(elementType);

            return GraphQLTypeDescription.List(elementTypeAdapter.GetTypeDescription(elementType));
        }
    }
}
