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

namespace WindupButton.GraphQL.Data
{
    internal sealed class Optional
    {
        public Optional()
            : this(null, false)
        {
        }

        public Optional(object value)
            : this(value, true)
        {
        }

        private Optional(object value, bool hasValue)
        {
            Value = value;
            HasValue = hasValue;
        }

        public object Value { get; }
        public bool HasValue { get; }

        public override string ToString()
        {
            return HasValue
                ? Value is string
                    ? $"\"{Value}\""
                    : Value == null
                        ? "null"
                        : Value.ToString()
                : "undefined";
        }
    }
}
