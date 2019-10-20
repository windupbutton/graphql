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
using System.Text.RegularExpressions;
using WindupButton.GraphQL.Data;

namespace WindupButton.GraphQL.Parsing
{
    internal sealed class Lexer
    {
        private static readonly Regex ellipsis = new Regex(@"^\.\.\.", RegexOptions.Compiled);
        private static readonly string punctuators = "!$():=@[]{}|";

        public IEnumerable<Token> Tokenize(string queryText)
        {
            return queryText.Split(new[] { "\r\n" }, StringSplitOptions.None)
                .SelectMany(x => x.Split("\r\n".ToCharArray()))
                .SelectMany((x, i) => Tokenize(i + 1, x))
                .ToList();
        }

        private static IEnumerable<Token> Tokenize(int lineNumber, string queryText)
        {
            return TokenizeStrings(lineNumber, queryText)
                .SelectMany(x => x is LexingToken token
                    ? Tokenize(token)
                    : new[] { x }
                )
                .ToList();
        }

        private static IEnumerable<Token> TokenizeStrings(int lineNumber, string queryText)
        {
            var tokens = new List<Token>();
            var inString = false;
            var start = 0;

            for (var i = 0; i < queryText.Length; ++i)
            {
                if (queryText[i] == '"')
                {
                    if (inString)
                    {
                        var tokenValue = queryText.Substring(start, i - start + 1);

                        if (!tokenValue.EndsWith("\\\"") || tokenValue.EndsWith("\\\\\""))
                        {
                            if (tokenValue.Length > 1)
                            {
                                tokens.Add(new StringValueToken(new GraphQLLocation(lineNumber, start + 1), tokenValue.Substring(1, tokenValue.Length - 2)));
                            }

                            start = i + 1;
                            inString = false;
                        }
                    }
                    else
                    {
                        var tokenValue = queryText.Substring(start, i - start);

                        if (tokenValue.Length > 0)
                        {
                            tokens.Add(new LexingToken(new GraphQLLocation(lineNumber, start + 1), tokenValue));
                        }

                        start = i;
                        inString = true;
                    }
                }
                else if (!inString && queryText[i] == '#')
                {
                    var tokenValue = queryText.Substring(start, i - start);

                    if (tokenValue.Length > 0)
                    {
                        tokens.Add(new LexingToken(new GraphQLLocation(lineNumber, start + 1), tokenValue));
                    }

                    start = i;
                    queryText = queryText.Substring(0, i);

                    break;
                }
            }

            if (inString)
            {
                throw new ParserException($"Unterminated string constant on line {lineNumber}, starting at col {start}");
            }

            var finalTokenValue = queryText.Substring(start);
            if (finalTokenValue.Length > 0)
            {
                tokens.Add(new LexingToken(new GraphQLLocation(lineNumber, start + 1), finalTokenValue));
            }

            return tokens;
        }

        private static IEnumerable<Token> Tokenize(LexingToken token)
        {
            var start = 0;

            for (var i = 0; i < token.Value.Length; ++i)
            {
                var c = token.Value[i];
                var index = punctuators.IndexOf(c);

                if (index >= 0)
                {
                    var nameValue = token.Value.Substring(start, i - start);

                    if (nameValue.Length > 0)
                    {
                        yield return Tokenize(new GraphQLLocation(token.Location.Line, token.Location.Column + start), nameValue);
                    }

                    yield return new PunctuatorToken(new GraphQLLocation(token.Location.Line, token.Location.Column + i), punctuators[index].ToString());

                    start = i + 1;
                }
                else if (ellipsis.IsMatch(token.Value, i))
                {
                    var nameValue = token.Value.Substring(start, i - start);

                    if (nameValue.Length > 0)
                    {
                        yield return Tokenize(new GraphQLLocation(token.Location.Line, token.Location.Column + start), nameValue);
                    }

                    yield return new PunctuatorToken(new GraphQLLocation(token.Location.Line, token.Location.Column + i), "...");

                    i += 2;
                    start = i + 1;
                }
                else if (c == ' ' || c == '\t' || c == ',')
                {
                    var nameValue = token.Value.Substring(start, i - start);

                    if (nameValue.Length > 0)
                    {
                        yield return Tokenize(new GraphQLLocation(token.Location.Line, token.Location.Column + start), nameValue);
                    }

                    start = i + 1;
                }
            }

            var lastNameValue = token.Value.Substring(start, token.Value.Length - start);

            if (lastNameValue.Length > 0)
            {
                yield return Tokenize(new GraphQLLocation(token.Location.Line, token.Location.Column + start), lastNameValue);
            }
        }

        private static Token Tokenize(GraphQLLocation location, string value)
        {
            if (int.TryParse(value, out var intValue))
            {
                return new IntValueToken(location, value, intValue);
            }
            if (decimal.TryParse(value, out var decimalValue))
            {
                return new FloatValueToken(location, value, decimalValue);
            }

            return new NameToken(location, value);
        }
    }
}
