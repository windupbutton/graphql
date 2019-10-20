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
using Newtonsoft.Json;
using WindupButton.GraphQL.Data;

namespace WindupButton.GraphQL.Schema
{
    public sealed class GraphQLInputObjectTypeAdapter : IGraphQLTypeAdapter
    {
        public CoercionResult CoerceInput(Type targetType, string name, object value)
        {
            Check.IsNotNull(targetType, nameof(targetType));

            if (!targetType.IsGenericType)
            {
                throw new InvalidOperationException(); // todo message
            }

            var wrappedType = targetType.GetGenericArguments()[0];

            var json = JsonConvert.SerializeObject(value);
            var converter = new InputConverter(name);
            var result = JsonConvert.DeserializeObject(json, wrappedType, converter);
            var errors = new List<CoercionError>(converter.Errors);

            var properties = result.GetType()
                .GetProperties()
                .Where(x => x.SetMethod != null && x.GetMethod != null && typeof(IInput).IsAssignableFrom(x.PropertyType));

            foreach (var property in properties)
            {
                if (property.GetValue(result) == null)
                {
                    var inputType = Input.FindInputType(property.PropertyType);

                    if (inputType == null)
                    {
                        throw new InvalidOperationException(); // todo message
                    }

                    var graphQLType = inputType.GetGenericArguments()[0];
                    var typeAdapter = GraphQLTypeHelper.CreateTypeAdapter(graphQLType);

                    if (typeAdapter == null)
                    {
                        throw new InvalidOperationException(); // todo message
                    }

                    var coercionResult = typeAdapter.CoerceInput(graphQLType, $"{name}.{property.Name}", null);
                    var propertyResult = Input.Create(property.PropertyType, coercionResult.Value);

                    errors.AddRange(coercionResult.Errors ?? Enumerable.Empty<CoercionError>());

                    property.SetValue(result, propertyResult);
                }
            }

            return new CoercionResult(result, errors);
        }

        public CoercionResult CoerceResult(Type targetType, string name, object value)
        {
            throw new InvalidOperationException(); // todo message
        }

        public GraphQLTypeDescription GetTypeDescription(Type targetType)
        {
            Check.IsNotNull(targetType, nameof(targetType));

            if (!targetType.IsGenericType)
            {
                throw new InvalidOperationException(); // todo message
            }

            var wrappedType = targetType.GetGenericArguments()[0];
            var wrappedTypeAdapter = GraphQLTypeHelper.CreateTypeAdapter(wrappedType);

            return wrappedTypeAdapter?.GetTypeDescription(wrappedType) ?? GraphQLTypeDescription.Named(wrappedType.Name);
        }
    }
}
