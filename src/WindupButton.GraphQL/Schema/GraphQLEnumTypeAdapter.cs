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
    public sealed class GraphQLEnumTypeAdapter : IGraphQLTypeAdapter
    {
        public CoercionResult CoerceInput(Type targetType, string name, object value)
        {
            Check.IsNotNull(targetType, nameof(targetType));

            if (!targetType.IsGenericType)
            {
                throw new InvalidOperationException(); // todo message
            }

            if (value == null)
            {
                return new CoercionResult(null);
            }

            var stringValue = value?.ToString();

            if (stringValue == null)
            {
                return new CoercionResult(null);
            }

            var enumType = targetType.GetGenericArguments()[0];
            var enumValue = Enum.GetValues(enumType)
                .OfType<object>()
                .Select(x => (value: x, description: enumType.Description(x.ToString())))
                .Where(x => x.description.Equals(stringValue, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.value)
                .FirstOrDefault()
                ?? Enum.GetValues(enumType)
                    .OfType<object>()
                    .Where(x => x.ToString().Equals(stringValue, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();

            return enumValue != null
                ? new CoercionResult(enumValue)
                : new CoercionResult(
                    null,
                    new[] { new CoercionError(name, GetTypeDescription(targetType), value) });
        }

        public CoercionResult CoerceResult(Type targetType, string name, object value)
        {
            Check.IsNotNull(targetType, nameof(targetType));

            if (!targetType.IsGenericType)
            {
                throw new InvalidOperationException(); // todo message
            }

            var stringValue = value?.ToString();

            if (stringValue == null)
            {
                return new CoercionResult(null);
            }

            var enumType = targetType.GetGenericArguments()[0];
            var enumValue = Enum.GetNames(enumType)
                .Select(x => enumType.Description(x))
                .FirstOrDefault(x => x.Equals(stringValue, StringComparison.InvariantCultureIgnoreCase))
                ?? Enum.GetNames(enumType)
                    .FirstOrDefault(x => x.Equals(stringValue, StringComparison.InvariantCultureIgnoreCase));

            return enumValue != null
                ? new CoercionResult(enumValue)
                : new CoercionResult(
                    null,
                    new[] { new CoercionError(name, GetTypeDescription(targetType), value) });
        }

        public GraphQLTypeDescription GetTypeDescription(Type targetType)
        {
            if (!targetType.IsGenericType)
            {
                throw new InvalidOperationException(); // todo message
            }

            var enumType = targetType.GetGenericArguments()[0];

            return GraphQLTypeDescription.Named(enumType.Name);
        }
    }
}
