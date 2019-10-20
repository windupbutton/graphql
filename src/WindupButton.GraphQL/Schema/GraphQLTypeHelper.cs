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

namespace WindupButton.GraphQL.Schema
{
    public static class GraphQLTypeHelper
    {
        public static IGraphQLTypeAdapter CreateTypeAdapter(Type graphQLType)
        {
            Check.IsNotNull(graphQLType, nameof(graphQLType));

            var attribute = graphQLType.GetCustomAttribute<GraphQLTypeAttribute>(true);

            return attribute == null
                ? null
                : Activator.CreateInstance(attribute.AdapterType) as IGraphQLTypeAdapter;
        }
    }
}
