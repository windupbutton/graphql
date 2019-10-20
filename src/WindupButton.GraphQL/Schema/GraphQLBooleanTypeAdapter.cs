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

namespace WindupButton.GraphQL.Schema
{
    public sealed class GraphQLBooleanTypeAdapter : IGraphQLTypeAdapter
    {
        public CoercionResult CoerceInput(Type targetType, string name, object value)
        {
            if (value == null)
            {
                return new CoercionResult(null);
            }

            return value is bool boolValue
                ? new CoercionResult(boolValue, null)
                : new CoercionResult(
                    null,
                    new[] { new CoercionError(name, GetTypeDescription(targetType), value) });
        }

        public CoercionResult CoerceResult(Type targetType, string name, object value)
        {
            if (value == null)
            {
                return new CoercionResult(null);
            }

            if (value is bool boolValue)
            {
                return new CoercionResult(boolValue);
            }

            if (value is int intValue)
            {
                return new CoercionResult(intValue != 0);
            }

            if (value is long longValue)
            {
                return new CoercionResult(longValue != 0);
            }

            return new CoercionResult(null, new[] { new CoercionError(name, GetTypeDescription(targetType), value) });
        }

        public GraphQLTypeDescription GetTypeDescription(Type targetType)
        {
            return GraphQLTypeDescription.Named("Boolean");
        }
    }
}
