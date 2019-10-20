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

namespace WindupButton.GraphQL.Ast
{
    internal sealed class NamedType : InputType
    {
        public NamedType(string name)
        {
            Check.IsNotNullOrWhiteSpace(name, nameof(name));

            Name = name;
        }

        public string Name { get; }

        public static InputType Int { get; } = new NamedType("Int");
        public static InputType String { get; } = new NamedType("String");
        public static InputType Float { get; } = new NamedType("Float");
        public static InputType Boolean { get; } = new NamedType("Boolean");

        public override GraphQLTypeDescription GetTypeDescription()
        {
            return GraphQLTypeDescription.Named(Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is NamedType namedType)
            {
                return Name.Equals(namedType.Name);
            }

            return base.Equals(obj);
        }
    }
}
