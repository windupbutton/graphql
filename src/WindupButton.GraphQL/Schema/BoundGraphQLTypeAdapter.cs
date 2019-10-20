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
    internal sealed class BoundGraphQLTypeAdapter
    {
        private readonly Type targetType;
        private readonly IGraphQLTypeAdapter graphQLTypeAdapter;

        public BoundGraphQLTypeAdapter(Type targetType, IGraphQLTypeAdapter graphQLTypeAdapter)
        {
            Check.IsNotNull(targetType, nameof(targetType));
            Check.IsNotNull(graphQLTypeAdapter, nameof(graphQLTypeAdapter));

            this.targetType = targetType;
            this.graphQLTypeAdapter = graphQLTypeAdapter;
        }

        public CoercionResult CoerceInput(string name, object value)
        {
            return graphQLTypeAdapter.CoerceInput(targetType, name, value);
        }

        public CoercionResult CoerceResult(string name, object value)
        {
            return graphQLTypeAdapter.CoerceResult(targetType, name, value);
        }

        public GraphQLTypeDescription GetTypeDescription()
        {
            return graphQLTypeAdapter.GetTypeDescription(targetType);
        }
    }
}
