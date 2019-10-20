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
    public abstract class GraphQLTypeDescription
    {
        private GraphQLTypeDescription()
        {
        }

        public static GraphQLTypeDescription List(GraphQLTypeDescription graphQLType)
        {
            Check.IsNotNull(graphQLType, nameof(graphQLType));

            return new ListType(graphQLType);
        }

        public static GraphQLTypeDescription NotNull(GraphQLTypeDescription graphQLType)
        {
            Check.IsNotNull(graphQLType, nameof(graphQLType));

            return new NotNullType(graphQLType);
        }

        public static GraphQLTypeDescription Named(string name)
        {
            Check.IsNotNullOrWhiteSpace(name, nameof(name));

            return new NamedType(name);
        }

        public abstract bool CanBeDownCastTo(GraphQLTypeDescription graphQLType);

        private sealed class ListType : GraphQLTypeDescription
        {
            private readonly GraphQLTypeDescription wrappedType;

            public ListType(GraphQLTypeDescription wrappedType)
            {
                Check.IsNotNull(wrappedType, nameof(wrappedType));

                this.wrappedType = wrappedType;
            }

            public override bool CanBeDownCastTo(GraphQLTypeDescription graphQLType)
            {
                return graphQLType is ListType listType && wrappedType.CanBeDownCastTo(listType.wrappedType);
            }

            public override string ToString()
            {
                return $"[{wrappedType}]";
            }
        }

        private sealed class NotNullType : GraphQLTypeDescription
        {
            private readonly GraphQLTypeDescription wrappedType;

            public NotNullType(GraphQLTypeDescription wrappedType)
            {
                Check.IsNotNull(wrappedType, nameof(wrappedType));

                this.wrappedType = wrappedType;
            }

            public override bool CanBeDownCastTo(GraphQLTypeDescription graphQLType)
            {
                Check.IsNotNull(graphQLType, nameof(graphQLType));

                return graphQLType is NotNullType notNullType && wrappedType.CanBeDownCastTo(notNullType.wrappedType)
                    || graphQLType is NamedType namedType && wrappedType.CanBeDownCastTo(namedType);
            }

            public override string ToString()
            {
                return $"{wrappedType}!";
            }
        }

        private sealed class NamedType : GraphQLTypeDescription
        {
            private readonly string name;

            public NamedType(string name)
            {
                Check.IsNotNullOrWhiteSpace(name, nameof(name));

                this.name = name;
            }

            public override bool CanBeDownCastTo(GraphQLTypeDescription graphQLType)
            {
                return graphQLType is NamedType namedType && name.Equals(namedType?.name);
            }

            public override string ToString()
            {
                return name;
            }
        }
    }
}
