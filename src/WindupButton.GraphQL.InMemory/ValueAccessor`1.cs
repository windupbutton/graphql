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

namespace WindupButton.GraphQL.InMemory
{
    internal sealed class ValueAccessor<T> : IValueAccessor<T>
        where T : IGraphQLType
    {
        private readonly Func<object> root;
        private readonly Func<object, object> accessor;

        public ValueAccessor(T graphQLType, Func<object> root, Func<object, object> accessor)
        {
            Check.IsNotNull(accessor, nameof(accessor));

            GraphQLType = graphQLType;
            this.root = root;
            this.accessor = accessor;
        }

        public T GraphQLType { get; }

        IGraphQLType IValueAccessor.GraphQLType => GraphQLType;

        public object GetValue(DataReference parent)
        {
            if (parent == null)
            {
                parent = new DataReference(root());
            }

            return parent.Value == null ? null : accessor(parent.Value);
        }
    }
}
