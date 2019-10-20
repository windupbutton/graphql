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
using System.Linq;
using WindupButton.GraphQL.Schema;

namespace WindupButton.GraphQL.Data
{
    public static class Input
    {
        internal static IInput Create(Type type, object value)
        {
            Check.IsNotNull(type, nameof(type));

            return (IInput)Activator.CreateInstance(type, value);
        }

        internal static Type FindInputType(Type objectType)
        {
            for (var type = objectType; type != null; type = type.BaseType)
            {
                if (type.IsGenericType && typeof(Input<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
                {
                    return type;
                }
            }

            return null;
        }

        public static Input<T> Create<T>(object value)
            where T : IGraphQLType
        {
            var coercionResult = GraphQLTypeHelper.CreateTypeAdapter(typeof(T))
                .CoerceInput(typeof(T), "<Anonymous input>", value);

            if (coercionResult.Errors != null && coercionResult.Errors.Any())
            {
                throw new InvalidOperationException(); // todo message (from coercionResult)
            }

            return new Input<T>(coercionResult.Value);
        }
    }
}
