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

namespace WindupButton.GraphQL.Data
{
    public sealed class CoercionError
    {
        public CoercionError(string name, GraphQLTypeDescription expected)
            : this(name, expected, null, false)
        {
        }

        public CoercionError(string name, GraphQLTypeDescription expected, object actual)
            : this(name, expected, actual, true)
        {
        }

        private CoercionError(string name, GraphQLTypeDescription expected, object actual, bool hasActual)
        {
            Check.IsNotNullOrWhiteSpace(name, nameof(name));
            Check.IsNotNull(expected, nameof(expected));

            Name = name;
            Expected = expected;
            Actual = actual;
        }

        public string Name { get; }
        public GraphQLTypeDescription Expected { get; }
        public object Actual { get; }
        public bool HasActual { get; }

        public override string ToString()
        {
            return $"Reason: '{Name}' {Expected} type expected{(HasActual ? $" but got {FormatActual()}" : "")}";
        }

        private string FormatActual()
        {
            return Actual is string
                ? $"\"{Actual}\""
                : Actual == null
                ? "null"
                : Actual.ToString();
        }
    }
}
