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
using WindupButton.GraphQL.Data;

namespace WindupButton.GraphQL.Ast
{
    internal sealed class Field : Selection
    {
        public Field(
            string name,
            string alias,
            IEnumerable<Argument> arguments,
            IEnumerable<Directive> directives,
            IEnumerable<Selection> selectionSet,
            GraphQLLocation location)
        {
            Check.IsNotNullOrWhiteSpace(name, nameof(name));
            Check.IsNotNull(location, nameof(location));

            Name = name;
            Alias = alias;
            Arguments = arguments;
            Directives = directives;
            SelectionSet = selectionSet;
            Location = location;
        }

        public string Name { get; }
        public string Alias { get; }
        public IEnumerable<Argument> Arguments { get; }
        public IEnumerable<Directive> Directives { get; }
        public IEnumerable<Selection> SelectionSet { get; }

        public GraphQLLocation Location { get; }

        public override int GetHashCode()
        {
            var result = Name.GetHashCode();

            if (Alias != null)
            {
                result ^= Alias.GetHashCode();
            }

            if (Arguments != null)
            {
                result = Arguments.Aggregate(result, (x, y) => x ^ y.GetHashCode());
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
            if (obj is Field field)
            {
                return
                    Name.Equals(field.Name) &&
                    (
                        Arguments == null &&
                        field.Arguments == null ||
                        Arguments != null &&
                        field.Arguments != null &&
                        Arguments.SequenceEqual(field.Arguments)
                    ) &&
                    (
                        Directives == null &&
                        field.Directives == null ||
                        Directives != null &&
                        field.Directives != null &&
                        Directives.SequenceEqual(field.Directives)
                    ) &&
                    (
                        SelectionSet == null &&
                        field.SelectionSet == null ||
                        SelectionSet != null &&
                        field.SelectionSet != null &&
                        SelectionSet.SequenceEqual(field.SelectionSet)
                    );
            }

            return base.Equals(obj);
        }
    }
}
