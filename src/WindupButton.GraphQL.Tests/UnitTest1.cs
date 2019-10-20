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
using System.Diagnostics;
using Newtonsoft.Json;
using WindupButton.GraphQL.Data;
using WindupButton.GraphQL.Schema;
using Xunit;

namespace WindupButton.GraphQL.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var child1 = new Child1();
            var child2 = new Child2();
            var child3 = new Child3();

            Debugger.Break();
        }

        [Fact]
        public void Test2()
        {
            var value = new
            {
                StringValue = "string value",
                //Items = new[]
                //{
                //    "Item 1",
                //    "Item 2",
                //},
                Items = new[]
                {
                    new
                    {
                        StringValue = "string value 1",
                    },
                    new
                    {
                        StringValue = "string value 2",
                    },
                }
            };
            var json = JsonConvert.SerializeObject(value);
            var adapter = new GraphQLInputObjectTypeAdapter();
            var result = adapter.CoerceInput(typeof(GraphQLInputObject<TestInput>), "name", value);
        }

        [Fact]
        public void Test3()
        {
            var value = Input.Create<GraphQLNotNull<GraphQLList<GraphQLNotNull<GraphQLFloat>>>>(new[] { 1m, 2m });

            var a = value.Value();
            var b = value.ListValue();
        }
    }

    internal sealed class TestInput
    {
        public Input<GraphQLString> StringValue { get; set; }

        //public Input<GraphQLNotNull<GraphQLList<GraphQLNotNull<GraphQLString>>>> Items { get; set; }
        public Input<GraphQLList<GraphQLInputObject<TestChildInput>>> Items { get; set; }
    }

    internal sealed class TestChildInput
    {
        public Input<GraphQLString> StringValue { get; set; }
    }

    internal abstract class Base<T>
    {
        private static List<string> values = new List<string>();

        public List<string> Values => values;

        protected static void Add(string value)
        {
            values.Add(value);
        }
    }

    internal sealed class Child1 : Base<object>
    {
        static Child1()
        {
            Add("child 1");
        }
    }

    internal sealed class Child2 : Base<object>
    {
        static Child2()
        {
            Add("child 2");
        }
    }

    internal sealed class Child3 : Base<string>
    {
        static Child3()
        {
            Add("child 3");
        }
    }
}
