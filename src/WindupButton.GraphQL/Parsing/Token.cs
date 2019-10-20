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

using System.Diagnostics;
using WindupButton.GraphQL.Data;

namespace WindupButton.GraphQL.Parsing
{
    [DebuggerDisplay("[{GetType().Name}] ({Location.Line}, {Location.Column}) {Value}")]
    internal abstract class Token
    {
        public Token(GraphQLLocation location, string value)
        {
            Check.IsNotNull(location, nameof(location));
            Check.IsNotNull(value, nameof(value));

            Location = location;
            Value = value;
        }

        public GraphQLLocation Location { get; }
        public string Value { get; }

        public override int GetHashCode()
        {
            return Value.GetHashCode() ^ Location.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Token token)
            {
                return
                    token.Value.Equals(Value) &&
                    token.Location.Equals(Location);
            }

            return base.Equals(obj);
        }
    }
}
