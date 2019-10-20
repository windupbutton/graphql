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
    internal sealed class Directive
    {
        public Directive(string name, IEnumerable<Argument> arguments)
        {
            Check.IsNotNullOrWhiteSpace(name, nameof(name));

            Name = name;
            Arguments = arguments;
        }

        public string Name { get; }
        public IEnumerable<Argument> Arguments { get; }

        public override int GetHashCode()
        {
            var result = Name.GetHashCode();

            if (Arguments != null)
            {
                result = Arguments.Aggregate(result, (x, y) => x ^ y.GetHashCode());
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is Directive directive)
            {
                return Name.Equals(directive.Name) &&
                    (
                        Arguments == null &&
                        directive.Arguments == null ||
                        Arguments != null &&
                        directive.Arguments != null &&
                        Arguments.SequenceEqual(directive.Arguments)
                    );
            }

            return base.Equals(obj);
        }
    }
}
