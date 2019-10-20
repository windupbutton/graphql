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
using System.Linq;
using WindupButton.GraphQL.Data;

namespace WindupButton.GraphQL.Schema
{
    public sealed class GraphQLNotNullTypeAdapter : IGraphQLTypeAdapter
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

            var coercedValue = elementTypeAdapter
                .CoerceInput(elementType, name, value);

            return coercedValue.Errors != null && coercedValue.Errors.Any()
                ? coercedValue
                : new CoercionResult(
                    coercedValue.Value,
                    coercedValue.Value == null
                        ? new[] { new CoercionError(name, GetTypeDescription(targetType), value) }
                        : null);
        }

        public CoercionResult CoerceResult(Type targetType, string name, object value)
        {
            Check.IsNotNull(targetType, nameof(targetType));

            if (!targetType.IsGenericType)
            {
                throw new InvalidOperationException(); // todo message
            }

            var elementType = targetType.GetGenericArguments()[0];
            var elementTypeAdapter = GraphQLTypeHelper.CreateTypeAdapter(elementType);

            var coercedValue = elementTypeAdapter
                .CoerceResult(elementType, name, value);

            return new CoercionResult(
                coercedValue.Value,
                coercedValue.Value == null
                    ? new[] { new CoercionError(name, GetTypeDescription(targetType), value) }
                    : null);
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

            return GraphQLTypeDescription.NotNull(elementTypeAdapter.GetTypeDescription(elementType));
        }
    }
}
