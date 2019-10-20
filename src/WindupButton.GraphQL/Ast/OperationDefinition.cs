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

namespace WindupButton.GraphQL.Ast
{
    internal sealed class OperationDefinition : Definition
    {
        public OperationDefinition(
            OperationType operationType,
            string name,
            IEnumerable<VariableDefinition> variableDefinitions,
            IEnumerable<Directive> directives,
            IEnumerable<Selection> selectionSet)
        {
            Check.That(Enum.IsDefined(typeof(OperationType), operationType), "Invalid enum value", nameof(operationType));

            OperationType = operationType;
            Name = name;
            VariableDefinitions = variableDefinitions;
            Directives = directives;
            SelectionSet = selectionSet;
        }

        public OperationType OperationType { get; }
        public string Name { get; }
        public IEnumerable<VariableDefinition> VariableDefinitions { get; }
        public IEnumerable<Directive> Directives { get; }
        public IEnumerable<Selection> SelectionSet { get; }

        public override int GetHashCode()
        {
            var result = OperationType.GetHashCode();

            if (Name != null)
            {
                result ^= Name.GetHashCode();
            }

            if (VariableDefinitions != null)
            {
                result = VariableDefinitions.Aggregate(result, (x, y) => x ^ y.GetHashCode());
            }

            if (Directives != null)
            {
                result = Directives.Aggregate(result, (x, y) => x ^ y.GetHashCode());
            }

            if (SelectionSet != null)
            {
                result = SelectionSet.Aggregate(result, (x, y) => x ^ y.GetHashCode());
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is OperationDefinition operationDefinition)
            {
                return
                    OperationType.Equals(operationDefinition.OperationType) &&
                    (
                        Name == null &&
                        operationDefinition.Name == null ||
                        Name != null &&
                        operationDefinition.Name != null &&
                        Name.Equals(operationDefinition.Name)
                    ) &&
                    (
                        VariableDefinitions == null &&
                        operationDefinition.VariableDefinitions == null ||
                        VariableDefinitions != null &&
                        operationDefinition.VariableDefinitions != null &&
                        VariableDefinitions.SequenceEqual(operationDefinition.VariableDefinitions)
                    ) &&
                    (
                        Directives == null &&
                        operationDefinition.Directives == null ||
                        Directives != null &&
                        operationDefinition.Directives != null &&
                        Directives.SequenceEqual(operationDefinition.Directives)
                    ) &&
                    (
                        SelectionSet == null &&
                        operationDefinition.SelectionSet == null ||
                        SelectionSet != null &&
                        operationDefinition.SelectionSet != null &&
                        SelectionSet.SequenceEqual(operationDefinition.SelectionSet)
                    );
            }

            return base.Equals(obj);
        }
    }
}
