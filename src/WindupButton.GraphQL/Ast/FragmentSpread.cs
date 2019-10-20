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
    internal sealed class FragmentSpread : Selection
    {
        public FragmentSpread(string fragmentName, IEnumerable<Directive> directives)
        {
            Check.IsNotNullOrWhiteSpace(fragmentName, nameof(fragmentName));

            FragmentName = fragmentName;
            Directives = directives;
        }

        public string FragmentName { get; }
        public IEnumerable<Directive> Directives { get; }

        public override int GetHashCode()
        {
            var result = FragmentName.GetHashCode();

            if (Directives != null)
            {
                result = Directives.Aggregate(result, (x, y) => x ^ y.GetHashCode());
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is FragmentSpread fragmentSpread)
            {
                return
                    FragmentName.Equals(fragmentSpread.FragmentName) &&
                    (
                        Directives == null &&
                        fragmentSpread.Directives == null ||
                        Directives != null &&
                        fragmentSpread.Directives != null &&
                        Directives.SequenceEqual(fragmentSpread.Directives)
                    );
            }

            return base.Equals(obj);
        }
    }
}
