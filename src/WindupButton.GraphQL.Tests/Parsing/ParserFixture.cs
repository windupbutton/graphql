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

using System.Linq;
using WindupButton.GraphQL.Ast;
using WindupButton.GraphQL.Data;
using WindupButton.GraphQL.Parsing;
using Xunit;

namespace WindupButton.GraphQL.Tests.Parsing
{
    public sealed class ParserFixture
    {
        [Fact]
        public void CanParseEmptyQuery()
        {
            //arrange
            var query = "";
            var lexer = new Lexer();
            var parser = new Parser();

            //act
            var document = parser.Parse(lexer.Tokenize(query));

            //assert
            Assert.Empty(document.Definitions);
        }

        [Fact]
        public void CanParseSimpleQuery()
        {
            //arrange
            var query = @"
{
    foo
    bar
    baz
}";
            var lexer = new Lexer();
            var parser = new Parser();

            //act
            var document = parser.Parse(lexer.Tokenize(query));

            //assert
            Assert.Equal(
                new Document(new Definition[] {
                    new OperationDefinition(
                        OperationType.Query,
                        null,
                        null,
                        null,
                        new[]
                        {
                            new Field("foo",null, null, null, null, new GraphQLLocation(0, 0)),
                            new Field("bar",null, null, null, null, new GraphQLLocation(0, 0)),
                            new Field("baz",null, null, null, null, new GraphQLLocation(0, 0)),
                        }
                    )
                }),
                document
            );
        }

        [Fact]
        public void CanParseNestedQuery()
        {
            //arrange
            var query = @"
{
    foo {
        bar
        baz
    }
}";
            var lexer = new Lexer();
            var parser = new Parser();

            //act
            var document = parser.Parse(lexer.Tokenize(query));

            //assert
            Assert.Equal(
                new Document(new Definition[] {
                    new OperationDefinition(
                        OperationType.Query,
                        null,
                        null,
                        null,
                        new[]
                        {
                            new Field("foo", null, null, null, new[]
                            {
                                new Field("bar", null, null, null, null, new GraphQLLocation(0, 0)),
                                new Field("baz", null, null, null, null, new GraphQLLocation(0, 0)),
                            }, new GraphQLLocation(0, 0)),
                        }
                    )
                }),
                document
            );
        }

        [Fact]
        public void CanParseQueryWhereFieldsHaveParameters()
        {
            //arrange
            var query = @"
{
    foo(id: 123, name: ""name"") {
        one
        two
    }
    bar
    baz
}";
            var lexer = new Lexer();
            var parser = new Parser();

            //act
            var document = parser.Parse(lexer.Tokenize(query));

            //assert
            Assert.Equal(
                new Document(new Definition[] {
                    new OperationDefinition(
                        OperationType.Query,
                        null,
                        null,
                        null,
                        new[]
                        {
                            new Field(
                                "foo",
                                null,
                                new []
                                {
                                    new Argument("id", new GraphQLLocation(3, 9), new IntValue(123)),
                                    new Argument("name", new GraphQLLocation(3, 18), new StringValue("name")),
                                },
                                null,
                                new[]
                                {
                                    new Field("one", null, null, null, null, new GraphQLLocation(0, 0)),
                                    new Field("two", null, null, null, null, new GraphQLLocation(0, 0)),
                                }, new GraphQLLocation(0, 0)
                            ),
                            new Field("bar", null, null, null, null, new GraphQLLocation(0, 0)),
                            new Field("baz", null, null, null, null, new GraphQLLocation(0, 0)),
                        }
                    )
                }),
                document
            );
        }

        [Fact]
        public void CanParseNamedParameters()
        {
            //arrange
            var query = @"
query foo($id: Int) {
    foo(id: $id) {
        one
        two
    }
    bar
    baz
}";
            var result = new Document(new Definition[] {
                new OperationDefinition(
                    OperationType.Query,
                    "foo",
                    new[] { new VariableDefinition("id", new NamedType("Int"), null, new GraphQLLocation(0, 0)) },
                    null,
                    new[]
                    {
                        new Field(
                            "foo",
                            null,
                            new[]
                            {
                                new Argument("id", new GraphQLLocation(0, 0), new Variable("id", new GraphQLLocation(0, 0))),
                            },
                            null,
                            new[]
                            {
                                new Field("one", null, null, null, null, new GraphQLLocation(0, 0)),
                                new Field("two", null, null, null, null, new GraphQLLocation(0, 0)),
                            }, new GraphQLLocation(0, 0)
                        ),
                        new Field("bar", null, null, null, null, new GraphQLLocation(0, 0)),
                        new Field("baz", null, null, null, null, new GraphQLLocation(0, 0)),
                    }
                )
            });
            var lexer = new Lexer();
            var parser = new Parser();

            //act
            var document = parser.Parse(lexer.Tokenize(query));

            //assert
            Assert.Equal(result, document);
        }

        [Fact]
        public void CanParseObject()
        {
            //arrange
            var query = "{ foo(bar: { baz: 1 a: { b: 3 } }) { moo } }";
            var lexer = new Lexer();
            var parser = new Parser();

            //act
            var document = parser.Parse(lexer.Tokenize(query));

            //assert
            Assert.Equal(
                new Field(
                    "foo",
                    null,
                    new[]
                    {
                        new Argument(
                            "bar",
                            new GraphQLLocation(0, 0),
                            new ObjectValue(new[]
                            {
                                new ObjectField("baz", new IntValue(1)),
                                new ObjectField(
                                    "a",
                                    new ObjectValue(new[]
                                    {
                                        new ObjectField("b", new IntValue(3)),
                                    })
                                ),
                            })
                        )
                    },
                    null,
                    new[]
                    {
                        new Field("moo", null, null, null, null, new GraphQLLocation(0, 0)),
                    }, new GraphQLLocation(0, 0)
                ),
                document.Definitions.OfType<OperationDefinition>().First().SelectionSet.First()
            );
        }

        [Fact]
        public void CanParseArray()
        {
            //arrange
            var query = "{ foo(bar: [ 1, \"2\" 3 { a: \"b\" c: [ 4.5 ] } [ 6 7 ] ]) { moo } }";
            var lexer = new Lexer();
            var parser = new Parser();

            //act
            var document = parser.Parse(lexer.Tokenize(query));

            //assert
            Assert.Equal(
                new Field(
                    "foo",
                    null,
                    new[]
                    {
                        new Argument(
                            "bar",
                            new GraphQLLocation(0, 0),
                            new ListValue(new Value[]
                            {
                                new IntValue(1),
                                new StringValue("2"),
                                new IntValue(3),
                                new ObjectValue(new[]
                                {
                                    new ObjectField("a", new StringValue("b")),
                                    new ObjectField("c", new ListValue(new Value[]{ new FloatValue(4.5m) })),
                                }),
                                new ListValue(new[]
                                {
                                    new IntValue(6),
                                    new IntValue(7),
                                }),
                            })
                        ),
                    },
                    null,
                    new[]
                    {
                        new Field("moo", null, null, null, null, new GraphQLLocation(0, 0)),
                    }, new GraphQLLocation(0, 0)
                ),
                document.Definitions.OfType<OperationDefinition>().First().SelectionSet.First()
            );
        }

        [Fact]
        public void CanParseNonNullListNonNullVariable()
        {
            // arrange
            var query = "mutation($foo: [Bar!]!) {  }";
            var lexer = new Lexer();
            var parser = new Parser();

            // act
            var document = parser.Parse(lexer.Tokenize(query));

            // assert
        }

        [Fact]
        public void CanParseListNonNullVariable()
        {
            // arrange
            var query = "mutation($foo: [Bar!]) {  }";
            var lexer = new Lexer();
            var parser = new Parser();

            // act
            var document = parser.Parse(lexer.Tokenize(query));

            // assert
        }

        [Fact]
        public void CanParseNonNullListVariable()
        {
            // arrange
            var query = "mutation($foo: [Bar]!) {  }";
            var lexer = new Lexer();
            var parser = new Parser();

            // act
            var document = parser.Parse(lexer.Tokenize(query));

            // assert
        }

        [Fact]
        public void CanParseListVariable()
        {
            // arrange
            var query = "mutation($foo: [Bar]) {  }";
            var lexer = new Lexer();
            var parser = new Parser();

            // act
            var document = parser.Parse(lexer.Tokenize(query));

            // assert
        }

        [Fact]
        public void CanParseEnum()
        {
            // arrange
            var query = "query($foo: SomeEnum!) {  }";
            var lexer = new Lexer();
            var parser = new Parser();

            // act
            var document = parser.Parse(lexer.Tokenize(query));

            // assert
        }

        [Fact]
        public void CanParseConstant()
        {
            // arrange
            var query = "query() { Foo(value: \"value\") }";
            var lexer = new Lexer();
            var parser = new Parser();

            // act
            var document = parser.Parse(lexer.Tokenize(query));

            // assert
        }
    }
}
