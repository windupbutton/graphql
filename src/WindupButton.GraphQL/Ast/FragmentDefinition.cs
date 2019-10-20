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

namespace WindupButton.GraphQL.Ast
{
    internal sealed class FragmentDefinition : Definition
    {
        public FragmentDefinition(string fragmentName, string typeCondition, IEnumerable<Directive> directives, IEnumerable<Selection> selectionSet)
        {
            Check.IsNotNullOrWhiteSpace(fragmentName, nameof(fragmentName));
            Check.IsNotNullOrWhiteSpace(typeCondition, nameof(typeCondition));
            Check.IsNotNull(selectionSet, nameof(selectionSet));

            FragmentName = fragmentName;
            TypeCondition = typeCondition;
            Directives = directives;
            SelectionSet = selectionSet;
        }

        public string FragmentName { get; }
        public string TypeCondition { get; }
        public IEnumerable<Directive> Directives { get; }
        public IEnumerable<Selection> SelectionSet { get; }

        public override int GetHashCode()
        {
            var result = FragmentName.GetHashCode() ^ TypeCondition.GetHashCode();

            if (Directives != null)
            {
                result = Directives.Aggregate(result, (x, y) => x ^ y.GetHashCode());
            }

            return SelectionSet.Aggregate(result, (x, y) => x ^ y.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj is FragmentDefinition fragmentDefinition)
            {
                return
                    FragmentName.Equals(fragmentDefinition.FragmentName) &&
                    TypeCondition.Equals(fragmentDefinition.TypeCondition) &&
                    (
                        Directives == null &&
                        fragmentDefinition.Directives == null ||
                        Directives != null &&
                        fragmentDefinition.Directives != null &&
                        Directives.SequenceEqual(fragmentDefinition.Directives)
                    ) &&
                    SelectionSet.SequenceEqual(fragmentDefinition.SelectionSet);
            }

            return base.Equals(obj);
        }
    }
}
