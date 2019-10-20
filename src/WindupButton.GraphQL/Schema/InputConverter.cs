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
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using WindupButton.GraphQL.Data;

namespace WindupButton.GraphQL.Schema
{
    internal class InputConverter : CustomCreationConverter<object>
    {
        private readonly string name;
        private readonly List<CoercionError> errors;

        public InputConverter(string name)
        {
            Check.IsNotNullOrWhiteSpace(name, nameof(name));

            this.name = name;
            errors = new List<CoercionError>();
        }

        public IEnumerable<CoercionError> Errors => errors;

        public override object Create(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IInput).IsAssignableFrom(objectType);

            //for (var type = objectType; type != null; type = type.BaseType)
            //{
            //    if (type.IsGenericType && typeof(Input<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
            //    {
            //        return true;
            //    }
            //}

            //return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var inputType = Input.FindInputType(objectType);

            if (inputType == null)
            {
                throw new InvalidOperationException(); // todo message
            }

            var graphQLType = inputType.GetGenericArguments()[0];
            var typeAdapter = GraphQLTypeHelper.CreateTypeAdapter(graphQLType);

            if (typeAdapter == null)
            {
                throw new InvalidOperationException(); // todo message
            }

            object value;
            if (reader.TokenType == JsonToken.StartArray)
            {
                var listValue = new List<object>();

                serializer.Populate(JArray.Load(reader).CreateReader(), listValue);

                value = listValue;
            }
            else
            {
                value = reader.Value;
            }

            //var value = reader.TokenType == JsonToken.StartArray
            //    ? JArray.Load(reader)
            //    : reader.Value;

            var coercionResult = typeAdapter.CoerceInput(graphQLType, $"{name}.{reader.Path}", value);
            var result = Input.Create(objectType, coercionResult.Value);

            errors.AddRange(coercionResult.Errors ?? Enumerable.Empty<CoercionError>());

            return result;
        }
    }
}
