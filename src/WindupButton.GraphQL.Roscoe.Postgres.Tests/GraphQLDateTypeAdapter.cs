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

namespace WindupButton.GraphQL.Roscoe.Postgres.Tests
{
    internal sealed class GraphQLDateTypeAdapter : IGraphQLTypeAdapter
    {
        public CoercionResult CoerceInput(Type targetType, string name, object value)
        {
            if (value == null)
            {
                return new CoercionResult(null);
            }

            return value is string stringValue && DateTime.TryParse(stringValue, out var date)
                ? new CoercionResult(date)
                : value is DateTime dateValue
                    ? new CoercionResult(dateValue)
                    : new CoercionResult(null, new[] { new CoercionError(name, GetTypeDescription(targetType), value) });
        }

        public CoercionResult CoerceResult(Type targetType, string name, object value)
        {
            if (value == null)
            {
                return new CoercionResult(null);
            }

            return value is string stringValue && DateTime.TryParse(stringValue, out var dateValue)
                ? new CoercionResult(dateValue)
                : value is DateTime date
                ? new CoercionResult(date)
                : new CoercionResult(null, new[] { new CoercionError(name, GetTypeDescription(targetType)) });
        }

        public GraphQLTypeDescription GetTypeDescription(Type targetType)
        {
            return GraphQLTypeDescription.Named("Date");
        }
    }
}
