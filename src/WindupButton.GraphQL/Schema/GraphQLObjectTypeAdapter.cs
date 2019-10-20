﻿// Copyright 2019 Windup Button
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
using System.Reflection;
using WindupButton.GraphQL.Data;

namespace WindupButton.GraphQL.Schema
{
    public sealed class GraphQLObjectTypeAdapter : IGraphQLTypeAdapter
    {
        public CoercionResult CoerceInput(Type targetType, string name, object value)
        {
            throw new InvalidOperationException(); // todo message
        }

        public CoercionResult CoerceResult(Type targetType, string name, object value)
        {
            // this is implemented because the wrapper types use it. Not strictly correct :)

            return new CoercionResult(value);
        }

        public GraphQLTypeDescription GetTypeDescription(Type targetType)
        {
            var displayAttribute = targetType.GetCustomAttribute<GraphQLObjectAttribute>();

            return GraphQLTypeDescription.Named(displayAttribute?.Name ?? targetType.Name);
        }
    }
}
