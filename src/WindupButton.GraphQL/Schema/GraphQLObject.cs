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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WindupButton.GraphQL.Data;

namespace WindupButton.GraphQL.Schema
{
    [GraphQLType(typeof(GraphQLObjectTypeAdapter))]
    public abstract class GraphQLObject : IGraphQLType
    {
        private readonly Dictionary<string, Field> fields;

        public GraphQLObject()
        {
            fields = new Dictionary<string, Field>();

            var fieldMembers = GetType()
                .GetTypeInfo()
                .GetMethods()
                .Select(x => new
                {
                    FieldAttribute = x.GetCustomAttribute<FieldAttribute>(),
                    ObsoleteAttribute = x.GetCustomAttribute<ObsoleteAttribute>(),
                    Method = x,
                })
                .Where(x => x.FieldAttribute != null)
                .ToList();

            foreach (var member in fieldMembers)
            {
                var fieldName = member.FieldAttribute.Name ?? member.Method.Name;
                var returnType = member.Method.ReturnType;

                if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    returnType = returnType.GetGenericArguments()[0];
                }

                var genericParams = returnType.GetGenericArguments();

                // todo check return type is IValueAccessor
                if (genericParams.Length == 0)
                {
                    throw new InvalidOperationException(); // todo message
                }

                var fieldType = genericParams[0];
                var nullable = genericParams[0].IsGenericType && typeof(GraphQLNotNull<>).IsAssignableFrom(genericParams[0].GetGenericTypeDefinition());
                var singular = true;
                var itemNullable = false;

                genericParams = genericParams[0].GetGenericArguments();

                if (genericParams.Length != 0 && genericParams[0].IsGenericType)
                {
                    singular = !typeof(GraphQLList<>).IsAssignableFrom(genericParams[0].GetGenericTypeDefinition());

                    genericParams = genericParams[0].GetGenericArguments();

                    if (genericParams.Length != 0 && genericParams[0].IsGenericType)
                    {
                        itemNullable = typeof(GraphQLNotNull<>).IsAssignableFrom(genericParams[0].GetGenericTypeDefinition());
                    }
                }

                fields.Add(fieldName, new Field(
                    fieldName,
                    member.FieldAttribute.Description,
                    member.ObsoleteAttribute == null ? null : member.ObsoleteAttribute.Message ?? "No longer supported",
                    nullable,
                    singular,
                    itemNullable,
                    new BoundGraphQLTypeAdapter(fieldType, GraphQLTypeHelper.CreateTypeAdapter(fieldType)),
                    member.Method.GetParameters()
                        .Where(x => x.ParameterType.IsGenericType && x.ParameterType.GetGenericTypeDefinition() == typeof(Input<>))
                        .ToDictionary(
                            x => x.Name,
                            x => new BoundGraphQLTypeAdapter(
                                x.ParameterType.GetGenericArguments()[0],
                                GraphQLTypeHelper.CreateTypeAdapter(x.ParameterType.GetGenericArguments()[0]))), // todo gracefully fail. we're grabbing the GQL type from the generic parameter of Input<>
                    new ValueAccessorFactory(this, member.Method)));
            }
        }

        public IGraphQLTypeAdapter TypeAdapter => GraphQLTypeHelper.CreateTypeAdapter(GetType());

        internal ReadOnlyDictionary<string, Field> Fields => new ReadOnlyDictionary<string, Field>(fields);

        public IGraphQLType Unwrap()
        {
            return this;
        }
    }
}
