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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using WindupButton.GraphQL.Data;
using WindupButton.GraphQL.Schema;

namespace WindupButton.GraphQL.Roscoe.Postgres
{
    internal class ValueAccessor<T> : IValueAccessor<T>
        where T : IGraphQLType
    {
        private readonly IEnumerable<(string name, Type type)> fields;
        private readonly Func<object[], object> valueTransformer;

        private readonly Func<object> commandResultAccessor;

        public ValueAccessor(
            T graphQLObject,
            IEnumerable<(string name, Type type)> fields,
            Func<object[], object> valueTransformer,
            Func<object> commandResultAccessor)
        {
            Check.IsNotNull(graphQLObject, nameof(graphQLObject));
            Check.IsNotNull(fields, nameof(fields));
            Check.IsNotNull(valueTransformer, nameof(valueTransformer));
            Check.IsNotNull(commandResultAccessor, nameof(commandResultAccessor));

            GraphQLType = graphQLObject;

            this.fields = fields;
            this.valueTransformer = valueTransformer;

            this.commandResultAccessor = commandResultAccessor;
        }

        public ValueAccessor(T graphQLObject, string name, Type type, Func<object> commandResultAccessor)
            : this(graphQLObject, new[] { (name, type) }, x => x[0], commandResultAccessor)
        {
        }

        public T GraphQLType { get; }

        IGraphQLType IValueAccessor.GraphQLType => GraphQLType;

        public object GetValue(DataReference value)
        {
            if (value == null)
            {
                value = new DataReference(commandResultAccessor());
            }

            return Transform(value.Value);
        }

        private object Transform(object value)
        {
            return value == null
                ? null
                : value is JToken jToken
                    ? jToken.Type == JTokenType.Array
                        ? jToken.Values()
                            .Select(x => Transform(x))
                            .ToList()
                        : jToken.Type == JTokenType.Object
                            ? valueTransformer(TransformJObject(jToken.Value<JObject>()))
                            : jToken.Type == JTokenType.Property
                                ? valueTransformer(new[] { GetJTokenValue(jToken.Value<JProperty>().Value, fields.First().type) })
                                : throw new InvalidOperationException()
                    : value is IDictionary<string, object> dict
                        ? valueTransformer(fields.Select(x => dict.TryGetValue(x.name, out var dictValue) ? dictValue : null).ToArray())
                        : value is IEnumerable enumerable
                            ? enumerable
                                .OfType<object>()
                                .Select(x => Transform(x))
                                .ToList()
                            : throw new InvalidOperationException();

            object[] TransformJObject(JObject jObject) => fields
                .Select(x => jObject.TryGetValue(x.name, out var jObjectValue) ? GetJTokenValue(jObjectValue, x.type) : null)
                .ToArray();

            object GetJTokenValue(JToken token, Type type)
            {
                if (new[] { JTokenType.Array, JTokenType.Object }.Contains(token.Type))
                {
                    return token;
                }

                return type == null ? token.Value<object>() : token.ToObject(type);
            }
        }
    }
}
