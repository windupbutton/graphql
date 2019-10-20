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

using WindupButton.GraphQL.Data;
using WindupButton.GraphQL.Parsing;
using Xunit;

namespace WindupButton.GraphQL.Tests.Parsing
{
    public sealed class LexerFixture
    {
        [Fact]
        public void CanTokenizeEmptyString()
        {
            //arrange
            var query = "";
            var lexer = new Lexer();

            //act
            var tokens = lexer.Tokenize(query);

            //assert
            Assert.Empty(tokens);
        }

        [Fact]
        public void CanGetColNumbers()
        {
            //arrange
            var query = @"
  a: b  c   d";
            var lexer = new Lexer();

            //act
            var tokens = lexer.Tokenize(query);

            //assert
            Assert.Equal(
                new Token[]
                {
                    new NameToken(new GraphQLLocation(2,3),"a"),
                    new PunctuatorToken(new GraphQLLocation(2,4),":"),
                    new NameToken(new GraphQLLocation(2,6),"b"),
                    new NameToken(new GraphQLLocation(2,9),"c"),
                    new NameToken(new GraphQLLocation(2,13),"d"),
                },
                tokens
            );
        }

        [Fact]
        public void CanTokenizeEmptyQuery()
        {
            //arrange
            var query = "query Foo {\n}";
            var lexer = new Lexer();

            //act
            var tokens = lexer.Tokenize(query);

            //assert
            Assert.Equal(
                new Token[]
                {
                    new NameToken(new GraphQLLocation(1, 1),"query"),
                    new NameToken(new GraphQLLocation(1, 7),"Foo"),
                    new PunctuatorToken(new GraphQLLocation(1, 11),"{"),
                    new PunctuatorToken(new GraphQLLocation(2, 1),"}"),
                },
                tokens
            );
        }

        [Fact]
        public void CanTokenizeAllTokens()
        {
            //arrange
            var query = @"{
    string1: ""foo""
    string2: ""a\""b""
    int1 : 0, int2: -0
    int3: -1 int4: 123
    float1: 1.0
    float2: 1e1
    float3: 1.0e1
    float4: 1e+1
    float5: 1e-1
    float6: 1.0e+1
    float7: 1.0e-1
    float8: 1E1
    float9: 1.0E1
    float10: 1E+1
    float11: 1E-1
    float12: 1.0E+1
    float13: 1.0E-1
    bool1: true
    bool2: false
}";
            var lexer = new Lexer();

            //act
            var tokens = lexer.Tokenize(query);

            //assert
            Assert.Equal(
                new Token[]
                {
                    new PunctuatorToken(new GraphQLLocation(1, 1), "{"),

                    new NameToken(new GraphQLLocation(5, 2), "string1"),
                    new PunctuatorToken(new GraphQLLocation(12, 2), ":"),
                    new StringValueToken(new GraphQLLocation(14, 2), "foo"),

                    new NameToken(new GraphQLLocation(5, 3), "string2"),
                    new PunctuatorToken(new GraphQLLocation(12, 3), ":"),
                    new StringValueToken(new GraphQLLocation(14, 3), "a\\\"b"),

                    new NameToken(new GraphQLLocation(5, 4), "int1"),
                    new PunctuatorToken(new GraphQLLocation(10, 4), ":"),
                    new IntValueToken(new GraphQLLocation(4, 12), "0", 0),

                    new NameToken(new GraphQLLocation(15, 4), "int2"),
                    new PunctuatorToken(new GraphQLLocation(19, 4), ":"),
                    new IntValueToken(new GraphQLLocation(4, 21), "-0", 0),

                    new NameToken(new GraphQLLocation(5, 5), "int3"),
                    new PunctuatorToken(new GraphQLLocation(9, 5), ":"),
                    new IntValueToken(new GraphQLLocation(5, 11), "-1", -1),

                    new NameToken(new GraphQLLocation(14, 5), "int4"),
                    new PunctuatorToken(new GraphQLLocation(18, 5), ":"),
                    new IntValueToken(new GraphQLLocation(5, 20), "123", 123),

                    new NameToken(new GraphQLLocation(5, 6), "float1"),
                    new PunctuatorToken(new GraphQLLocation(11, 6), ":"),
                    new FloatValueToken(new GraphQLLocation(6, 13), "1.0", 1m),

                    new NameToken(new GraphQLLocation(5, 7), "float2"),
                    new PunctuatorToken(new GraphQLLocation(11, 7), ":"),
                    new FloatValueToken(new GraphQLLocation(7, 13), "1e1", 1e1m),

                    new NameToken(new GraphQLLocation(5, 8), "float3"),
                    new PunctuatorToken(new GraphQLLocation(11, 8), ":"),
                    new FloatValueToken(new GraphQLLocation(8, 13), "1.0e1", 1.0e1m),

                    new NameToken(new GraphQLLocation(5, 9), "float4"),
                    new PunctuatorToken(new GraphQLLocation(11, 9), ":"),
                    new FloatValueToken(new GraphQLLocation(9, 13), "1e+1", 1e+1m),

                    new NameToken(new GraphQLLocation(5, 10), "float5"),
                    new PunctuatorToken(new GraphQLLocation(11, 10), ":"),
                    new FloatValueToken(new GraphQLLocation(10, 13), "1e-1", 1e-1m),

                    new NameToken(new GraphQLLocation(5, 11), "float6"),
                    new PunctuatorToken(new GraphQLLocation(11, 11), ":"),
                    new FloatValueToken(new GraphQLLocation(11, 13), "1.0e+1", 1.0e+1m),

                    new NameToken(new GraphQLLocation(5, 12), "float7"),
                    new PunctuatorToken(new GraphQLLocation(11, 12), ":"),
                    new FloatValueToken(new GraphQLLocation(12, 13), "1.0e-1", 1.0e-1m),

                    new NameToken(new GraphQLLocation(5, 13), "float8"),
                    new PunctuatorToken(new GraphQLLocation(11, 13), ":"),
                    new FloatValueToken(new GraphQLLocation(13, 13), "1E1", 1E1m),

                    new NameToken(new GraphQLLocation(5, 14), "float9"),
                    new PunctuatorToken(new GraphQLLocation(11, 14), ":"),
                    new FloatValueToken(new GraphQLLocation(14, 13), "1.0E1", 1.0E1m),

                    new NameToken(new GraphQLLocation(5, 15), "float10"),
                    new PunctuatorToken(new GraphQLLocation(12, 15), ":"),
                    new FloatValueToken(new GraphQLLocation(15, 14), "1E+1", 1E+1m),

                    new NameToken(new GraphQLLocation(5, 16), "float11"),
                    new PunctuatorToken(new GraphQLLocation(12, 16), ":"),
                    new FloatValueToken(new GraphQLLocation(16, 14), "1E-1", 1E-1m),

                    new NameToken(new GraphQLLocation(5, 17), "float12"),
                    new PunctuatorToken(new GraphQLLocation(12, 17), ":"),
                    new FloatValueToken(new GraphQLLocation(17, 14), "1.0E+1", 1.0E+1m),

                    new NameToken(new GraphQLLocation(5, 18), "float13"),
                    new PunctuatorToken(new GraphQLLocation(12, 18), ":"),
                    new FloatValueToken(new GraphQLLocation(18, 14), "1.0E-1", 1.0E-1m),

                    new NameToken(new GraphQLLocation(5, 19), "bool1"),
                    new PunctuatorToken(new GraphQLLocation(10, 19), ":"),
                    new NameToken(new GraphQLLocation(12, 19), "true"),

                    new NameToken(new GraphQLLocation(5, 20), "bool2"),
                    new PunctuatorToken(new GraphQLLocation(10, 20), ":"),
                    new NameToken(new GraphQLLocation(12, 20), "false"),

                    new PunctuatorToken(new GraphQLLocation(1, 21), "}"),
                },
                tokens
            );
        }

        [Fact]
        public void CanTokenizeBraces()
        {
            //arrange
            var query = @"
{ foo(a: 123) }";
            var lexer = new Lexer();

            //act
            var tokens = lexer.Tokenize(query);

            //assert
            Assert.Equal(
                new Token[]
                {
                    new PunctuatorToken(new GraphQLLocation(1, 2), "{"),
                    new NameToken(new GraphQLLocation(3, 2), "foo"),
                    new PunctuatorToken(new GraphQLLocation(6, 2), "("),
                    new NameToken(new GraphQLLocation(7, 2), "a"),
                    new PunctuatorToken(new GraphQLLocation(8, 2), ":"),
                    new IntValueToken(new GraphQLLocation(2, 10), "123", 123),
                    new PunctuatorToken(new GraphQLLocation(13, 2), ")"),
                    new PunctuatorToken(new GraphQLLocation(15, 2), "}"),
                },
                tokens
            );
        }

        [Fact]
        public void CanTokenizeString()
        {
            //arrange
            var query = @"  ""foo:bar\""baz"":";
            var lexer = new Lexer();

            //act
            var tokens = lexer.Tokenize(query);

            //assert
            Assert.Equal(
                new Token[]
                {
                    new StringValueToken(new GraphQLLocation(1, 3), @"foo:bar\""baz"),
                    new PunctuatorToken(new GraphQLLocation(17, 1), ":"),
                },
                tokens
            );
        }

        [Fact]
        public void CanTokenizePunctuatorAfterString()
        {
            //arrange
            var query = @""""",";
            var lexer = new Lexer();

            //act
            var tokens = lexer.Tokenize(query);

            //assert
            Assert.Equal(
                new Token[]
                {
                    new StringValueToken(new GraphQLLocation( 1,  1), ""),
                },
                tokens
            );
        }
    }
}
