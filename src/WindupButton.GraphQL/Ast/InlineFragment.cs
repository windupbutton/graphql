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
    internal sealed class InlineFragment : Selection
    {
        public InlineFragment(string typeCondition, IEnumerable<Directive> directives, IEnumerable<Selection> selectionSet)
        {
            Check.IsNotNull(selectionSet, nameof(selectionSet));

            TypeCondition = typeCondition;
            Directives = directives;
            SelectionSet = selectionSet;
        }

        public string TypeCondition { get; }
        public IEnumerable<Directive> Directives { get; }
        public IEnumerable<Selection> SelectionSet { get; }

        public override int GetHashCode()
        {
            var result = 0;

            if (TypeCondition != null)
            {
                result = TypeCondition.GetHashCode();
            }

            if (Directives != null)
            {
                result = Directives.Aggregate(result, (x, y) => x ^ y.GetHashCode());
            }

            return SelectionSet.Aggregate(result, (x, y) => x ^ y.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj is InlineFragment inlineFragment)
            {
                return
                    (
                        TypeCondition == null &&
                        inlineFragment.TypeCondition == null ||
                        TypeCondition != null &&
                        inlineFragment.TypeCondition != null &&
                        TypeCondition.Equals(inlineFragment.TypeCondition)
                    ) &&
                    (
                        Directives == null &&
                        inlineFragment.Directives == null ||
                        Directives != null &&
                        inlineFragment.Directives != null &&
                        Directives.SequenceEqual(inlineFragment.Directives)
                    ) &&
                    SelectionSet.SequenceEqual(inlineFragment.SelectionSet);
            }

            return base.Equals(obj);
        }
    }
}
