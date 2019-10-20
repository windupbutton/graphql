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
using WindupButton.GraphQL.Data;
using WindupButton.GraphQL.Schema;

namespace WindupButton.GraphQL
{
    public static class ValueAccessorExtensions
    {
        public static IValueAccessor<TOther> Select<T, TOther>(this IValueAccessor<T> valueAccessor, Func<T, IValueAccessor<TOther>> selector)
            where T : IGraphQLType
            where TOther : IGraphQLType
        {
            Check.IsNotNull(valueAccessor, nameof(valueAccessor));
            Check.IsNotNull(selector, nameof(selector));

            return new SubSelectionValueAccessor<T, TOther>(valueAccessor, selector(valueAccessor.GraphQLType));
        }

        public static IValueAccessor<TOther> Select<T, TOther>(this IValueAccessor<GraphQLNotNull<T>> valueAccessor, Func<T, IValueAccessor<TOther>> selector)
            where T : IGraphQLType
            where TOther : IGraphQLType
        {
            Check.IsNotNull(valueAccessor, nameof(valueAccessor));
            Check.IsNotNull(selector, nameof(selector));

            return new SubSelectionValueAccessor<GraphQLNotNull<T>, TOther>(valueAccessor, selector(valueAccessor.GraphQLType.WrappedType));
        }
    }
}
