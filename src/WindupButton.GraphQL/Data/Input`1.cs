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

using WindupButton.GraphQL.Schema;

namespace WindupButton.GraphQL.Data
{
    public sealed class Input<TGraphQLType> : IInput<TGraphQLType>
        where TGraphQLType : IGraphQLType
    {
        private readonly object value;

        public Input(object value)
        {
            this.value = value;
        }

        object IInput.Value => value;

        public static implicit operator Input<TGraphQLType>(Input<GraphQLNotNull<TGraphQLType>> input)
        {
            return new Input<TGraphQLType>(input.value);
        }

        //public static implicit operator Input<GraphQLList<TGraphQLType>>(Input<GraphQLList<GraphQLNotNull<TGraphQLType>>> input)
        //{
        //    return new Input<GraphQLList<TGraphQLType>>(new GraphQLList<TGraphQLType>(input.graphQLType.WrappedType.WrappedType), input.Value);
        //}
    }
}
