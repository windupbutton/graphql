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
using System.Linq;

namespace WindupButton.GraphQL.Data
{
    public sealed class VariableValues
    {
        private readonly IDictionary<string, VariableValue> variables;

        public VariableValues(IDictionary<string, object> variables)
        {
            Check.IsNotNull(variables, nameof(variables));

            this.variables = variables
                .ToDictionary(x => x.Key, x => new VariableValue(x.Value));
        }

        public IEnumerable<string> UnusedVariables => variables
            .Where(x => !x.Value.HasBeenUsed)
            .Select(x => x.Key)
            .ToList();

        public bool TryGet(string key, out object value)
        {
            if (variables.TryGetValue(key, out var variableValue))
            {
                variableValue.HasBeenUsed = true;

                value = variableValue.Value;

                return true;
            }

            value = null;

            return false;
        }

        private sealed class VariableValue
        {
            public VariableValue(object value)
            {
                Value = value;
            }

            public object Value { get; }
            public bool HasBeenUsed { get; set; }
        }
    }
}
