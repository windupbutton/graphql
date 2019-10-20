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
    internal sealed class VariableDefinition
    {
        public VariableDefinition(string name, InputType type, Value defaultValue, GraphQLLocation location)
        {
            Check.IsNotNullOrWhiteSpace(name, nameof(name));
            Check.IsNotNull(type, nameof(type));
            Check.IsNotNull(location, nameof(location));

            Name = name;
            Type = type;
            DefaultValue = defaultValue;
            Location = location;
        }

        public string Name { get; }
        public InputType Type { get; }
        public Value DefaultValue { get; }
        public GraphQLLocation Location { get; }

        public override int GetHashCode()
        {
            var result = Name.GetHashCode() ^ Type.GetHashCode() ^ Location.GetHashCode();

            if (DefaultValue != null)
            {
                result ^= DefaultValue.GetHashCode();
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is VariableDefinition variableDefinition)
            {
                return
                    Name.Equals(variableDefinition.Name) &&
                    Type.Equals(variableDefinition.Type) &&
                    Location.Equals(variableDefinition.Location) &&
                    (
                        DefaultValue == null &&
                        variableDefinition.DefaultValue == null ||
                        DefaultValue != null &&
                        variableDefinition.DefaultValue != null &&
                        DefaultValue.Equals(variableDefinition.DefaultValue)
                    );
            }

            return base.Equals(obj);
        }
    }
}
