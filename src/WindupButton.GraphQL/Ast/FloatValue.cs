﻿// Copyright 2019 Windup Button
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
    internal sealed class FloatValue : Value
    {
        public FloatValue(decimal value)
        {
            Value = value;
        }

        public decimal Value { get; }

        public override InputType GetType(IDictionary<string, InputType> variableTypes)
        {
            return NamedType.Float;
        }

        public override Optional GetValue(VariableValues variables)
        {
            return new Optional(Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is FloatValue floatValue)
            {
                return Value.Equals(floatValue.Value);
            }

            return base.Equals(obj);
        }
    }
}
