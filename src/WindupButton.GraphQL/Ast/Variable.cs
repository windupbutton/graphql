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
using WindupButton.GraphQL.Data;

namespace WindupButton.GraphQL.Ast
{
    internal sealed class Variable : Value
    {
        public Variable(string name, GraphQLLocation location)
        {
            Check.IsNotNullOrWhiteSpace(name, nameof(name));
            Check.IsNotNull(location, nameof(location));

            Name = name;
            Location = location;
        }

        public string Name { get; }
        public GraphQLLocation Location { get; }

        public override InputType GetType(IDictionary<string, InputType> variableTypes)
        {
            return variableTypes.TryGetValue(Name, out var inputType) ? inputType : null;
        }

        public override Optional GetValue(VariableValues variables)
        {
            return variables.TryGet(Name, out var value)
                ? new Optional(value)
                : new Optional();
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Variable variable)
            {
                return Name.Equals(variable.Name);
            }

            return base.Equals(obj);
        }
    }
}
