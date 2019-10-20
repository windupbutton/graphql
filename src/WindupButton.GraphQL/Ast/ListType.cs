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
    internal sealed class ListType : InputType
    {
        public ListType(InputType type)
        {
            Check.IsNotNull(type, nameof(type));

            Type = type;
        }

        public InputType Type { get; }

        public override GraphQLTypeDescription GetTypeDescription()
        {
            return GraphQLTypeDescription.List(Type.GetTypeDescription());
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ListType listType)
            {
                return Type.Equals(listType.Type);
            }

            return base.Equals(obj);
        }
    }
}
