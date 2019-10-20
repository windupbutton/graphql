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

namespace WindupButton.GraphQL.Schema
{
    [GraphQLType(typeof(GraphQLNotNullTypeAdapter))]
    public sealed class GraphQLNotNull<TType> : IGraphQLNotNull<TType>
        where TType : IGraphQLType
    {
        public GraphQLNotNull(TType wrappedType)
        {
            Check.IsNotNull(wrappedType, nameof(wrappedType));

            WrappedType = wrappedType;
        }

        public IGraphQLTypeAdapter TypeAdapter => GraphQLTypeHelper.CreateTypeAdapter(GetType());

        internal TType WrappedType { get; }

        public IGraphQLType Unwrap() => WrappedType.Unwrap();
    }
}
