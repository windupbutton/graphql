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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WindupButton.GraphQL.Data;

namespace WindupButton.GraphQL.Schema
{
    internal sealed class Field
    {
        private readonly ValueAccessorFactory valueAccessorFactory;

        public Field(
            string name,
            string description,
            string deprecationReason,
            bool isNullable,
            bool isSingular,
            bool isItemNullable,
            BoundGraphQLTypeAdapter graphQLType,
            IDictionary<string, BoundGraphQLTypeAdapter> arguments,
            ValueAccessorFactory valueAccessorFactory)
        {
            Check.IsNotNullOrWhiteSpace(name, nameof(name));
            Check.IsNotNull(graphQLType, nameof(graphQLType));
            Check.IsNotNull(arguments, nameof(arguments));
            Check.IsNotNull(valueAccessorFactory, nameof(valueAccessorFactory));

            Name = name;
            Description = description;

            IsDeprecated = deprecationReason != null;
            DeprecationReason = deprecationReason;

            IsNullable = isNullable;
            IsSingular = isSingular;
            IsItemNullable = isItemNullable;

            GraphQLType = graphQLType;
            Arguments = arguments;

            this.valueAccessorFactory = valueAccessorFactory;
        }

        public string Name { get; }
        public string Description { get; }

        public bool IsDeprecated { get; }
        public string DeprecationReason { get; }

        public bool IsNullable { get; }
        public bool IsSingular { get; }
        public bool IsItemNullable { get; }

        public BoundGraphQLTypeAdapter GraphQLType { get; }
        public IDictionary<string, BoundGraphQLTypeAdapter> Arguments { get; }

        internal Task<IValueAccessor> Resolve(IDictionary<string, object> values, CancellationToken token)
        {
            return valueAccessorFactory.Create(values, token);
        }
    }
}
