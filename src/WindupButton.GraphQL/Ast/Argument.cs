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

namespace WindupButton.GraphQL.Ast
{
    internal sealed class Argument
    {
        public Argument(string name, GraphQLLocation location, Value value)
        {
            Check.IsNotNullOrWhiteSpace(name, nameof(name));

            Name = name;
            Location = location;
            Value = value;
        }

        public string Name { get; }
        public GraphQLLocation Location { get; }
        public Value Value { get; }

        public override int GetHashCode()
        {
            var result = Name.GetHashCode() ^ Location.GetHashCode();

            if (Value != null)
            {
                result ^= Value.GetHashCode();
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is Argument argument)
            {
                return Name.Equals(argument.Name)
                    && Location.Equals(argument.Location)
                    && (Value == null && argument.Value == null || Value != null && argument.Value != null && Value.Equals(argument.Value));
            }

            return base.Equals(obj);
        }
    }
}
