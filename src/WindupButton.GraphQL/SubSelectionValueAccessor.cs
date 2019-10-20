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

using WindupButton.GraphQL.Data;
using WindupButton.GraphQL.Schema;

namespace WindupButton.GraphQL
{
    internal sealed class SubSelectionValueAccessor<T, TOther> : IValueAccessor<TOther>
        where T : IGraphQLType
        where TOther : IGraphQLType
    {
        private readonly IValueAccessor<T> parentValueAccessor;
        private readonly IValueAccessor<TOther> childValueAccessor;

        public SubSelectionValueAccessor(IValueAccessor<T> parentValueAccessor, IValueAccessor<TOther> childValueAccessor)
        {
            Check.IsNotNull(parentValueAccessor, nameof(parentValueAccessor));
            Check.IsNotNull(childValueAccessor, nameof(childValueAccessor));

            this.parentValueAccessor = parentValueAccessor;
            this.childValueAccessor = childValueAccessor;
        }

        public TOther GraphQLType => childValueAccessor.GraphQLType;

        IGraphQLType IValueAccessor.GraphQLType => GraphQLType;

        public object GetValue(DataReference value)
        {
            var childValue = parentValueAccessor.GetValue(value);

            return childValueAccessor.GetValue(new DataReference(childValue));
        }
    }
}
