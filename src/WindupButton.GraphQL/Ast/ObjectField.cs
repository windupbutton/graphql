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

namespace WindupButton.GraphQL.Ast
{
    internal sealed class ObjectField
    {
        public ObjectField(string name, Value value)
        {
            Check.IsNotNullOrWhiteSpace(name, nameof(name));
            Check.IsNotNull(value, nameof(value));

            Name = name;
            Value = value;
        }

        public string Name { get; }
        public Value Value { get; }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ObjectField objectField)
            {
                return
                    Name.Equals(objectField.Name) &&
                    Value.Equals(objectField.Value);
            }

            return base.Equals(obj);
        }
    }
}
