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
using WindupButton.GraphQL.Ast;

namespace WindupButton.GraphQL.Data
{
    internal sealed class FieldSelection
    {
        public FieldSelection(
            string name,
            string alias,
            IEnumerable<Argument> arguments,
            IEnumerable<Directive> directives,
            IEnumerable<FieldSelection> selectionSet,
            Schema.Field field,
            GraphQLLocation location,
            IValueAccessor valueAccessor)
        {
            Check.IsNotNullOrWhiteSpace(name, nameof(name));
            Check.IsNotNull(field, nameof(field));
            Check.IsNotNull(location, nameof(location));
            Check.IsNotNull(valueAccessor, nameof(valueAccessor));

            Name = name;
            Alias = alias;
            Arguments = arguments;
            Directives = directives;
            SelectionSet = selectionSet;
            Field = field;
            Location = location;
            ValueAccessor = valueAccessor;
        }

        public string Name { get; }
        public string Alias { get; }
        public IEnumerable<Argument> Arguments { get; }
        public IEnumerable<Directive> Directives { get; }
        public IEnumerable<FieldSelection> SelectionSet { get; }

        public Schema.Field Field { get; }
        public GraphQLLocation Location { get; }

        public FieldSelection MergeWith(FieldSelection selectionField)
        {
            throw new NotImplementedException();
        }

        public IValueAccessor ValueAccessor { get; }
    }
}
