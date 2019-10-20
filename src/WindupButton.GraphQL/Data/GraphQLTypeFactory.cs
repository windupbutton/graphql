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
using WindupButton.GraphQL.Schema;

namespace WindupButton.GraphQL.Data
{
    internal static class GraphQLTypeFactory
    {
        public static IGraphQLType CreateGraphQLType(Type objectType)
        {
            var types = new List<Type>();

            for (var type = objectType.GenericTypeArguments[0]; type != null;)
            {
                types.Insert(0, type);

                type = type.IsGenericType
                    ? type.GenericTypeArguments[0]
                    : null;
            }

            return (IGraphQLType)types
                .Aggregate(new object[0], (x, y) => new object[] { Activator.CreateInstance(y, x) })
                .FirstOrDefault();
        }
    }
}
