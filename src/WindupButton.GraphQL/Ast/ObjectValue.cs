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
using System.Collections.Generic;
using System.Linq;
using WindupButton.GraphQL.Data;

namespace WindupButton.GraphQL.Ast
{
    internal sealed class ObjectValue : Value
    {
        public ObjectValue(IEnumerable<ObjectField> value)
        {
            Check.IsNotNull(value, nameof(value));

            Value = value;
        }

        public IEnumerable<ObjectField> Value { get; }

        public override InputType GetType(IDictionary<string, InputType> variableTypes)
        {
            throw new NotImplementedException(); // todo
        }

        public override Optional GetValue(VariableValues variables)
        {
            return new Optional(Value);
        }

        public override int GetHashCode()
        {
            return Value.Aggregate(0, (x, y) => x ^ y.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj is ObjectValue objectValue)
            {
                return Value.SequenceEqual(objectValue.Value);
            }

            return base.Equals(obj);
        }
    }
}
