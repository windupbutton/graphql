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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WindupButton.GraphQL.Ast;

namespace WindupButton.GraphQL.Parsing
{
    internal sealed class Parser
    {
        public Document Parse(IEnumerable<Token> tokens)
        {
            Check.IsNotNull(tokens, nameof(tokens));

            var iterator = new ListIterator<Token>(tokens.ToList());

            var definitions = ParseDefinitions(iterator)
                .ToList();

            return new Document(definitions);
        }

        private IEnumerable<Definition> ParseDefinitions(ListIterator<Token> iterator)
        {
            var token = iterator.FirstOrDefault();

            if (token == null)
            {
                yield break;
            }

            for (; ; )
            {
                if (token is NameToken)
                {
                    if (token.Value.Equals("query"))
                    {
                        token = AssertNotEof(iterator.FirstOrDefault());

                        yield return ParseQuery(ref token, iterator);
                    }
                    else if (token.Value.Equals("mutation"))
                    {
                        //AssertNotEof(iterator.FirstOrDefault());

                        yield return ParseMutation(iterator);
                    }
                    else if (token.Value.Equals("fragment"))
                    {
                        AssertNotEof(iterator.FirstOrDefault());

                        yield return ParseFragment(iterator);
                    }
                }
                else if (token is PunctuatorToken && token.Value.Equals("{"))
                {
                    yield return ParseQuery(ref token, iterator);
                }
                else
                {
                    throw new ParserException();
                }

                token = iterator.FirstOrDefault();

                if (token == null)
                {
                    break;
                }
            }
        }

        private static OperationDefinition ParseQuery(ref Token token, ListIterator<Token> iterator)
        {
            string operationName = null;

            if (token is NameToken)
            {
                operationName = token.Value;

                token = AssertNotEof(iterator.FirstOrDefault());
            }

            IEnumerable<VariableDefinition> variableDefinitions = null;

            if (token is PunctuatorToken && token.Value.Equals("("))
            {
                variableDefinitions = ParseVariableDefinitions(iterator)
                    .ToList();

                token = AssertNotEof(iterator.FirstOrDefault());
            }

            var directives = ParseDirectives(ref token, iterator);

            if (token is PunctuatorToken && token.Value.Equals("{"))
            {
                token = AssertNotEof(iterator.FirstOrDefault());

                var selectionSet = ParseSelectionSet(ref token, iterator);

                return new OperationDefinition(OperationType.Query, operationName, variableDefinitions, directives, selectionSet);
            }

            throw new ParserException($"Syntax error at {token.Location}. Expected 'query' or '{{', got {token.Value}");
        }

        private static OperationDefinition ParseMutation(ListIterator<Token> iterator)
        {
            var token = AssertNotEof(iterator.FirstOrDefault());
            string operationName = null;

            if (token is NameToken)
            {
                operationName = token.Value;

                token = AssertNotEof(iterator.FirstOrDefault());
            }

            var variableDefinitions = Enumerable.Empty<VariableDefinition>();

            if (token is PunctuatorToken && token.Value.Equals("("))
            {
                variableDefinitions = ParseVariableDefinitions(iterator)
                    .ToList();

                token = AssertNotEof(iterator.FirstOrDefault());
            }

            var directives = ParseDirectives(ref token, iterator);

            if (token is PunctuatorToken && token.Value.Equals("{"))
            {
                token = AssertNotEof(iterator.FirstOrDefault());

                var selectionSet = ParseSelectionSet(ref token, iterator);

                return new OperationDefinition(OperationType.Mutation, operationName, variableDefinitions, directives, selectionSet);
            }

            throw new ParserException($"Syntax error at {token.Location}. Expected 'mutation', got {token.Value}");
        }

        private static FragmentDefinition ParseFragment(ListIterator<Token> iterator)
        {
            var token = AssertNotEof(iterator.FirstOrDefault());

            if (!(token is NameToken))
            {
                throw new ParserException();
            }

            var name = token.Value;

            if (name.Equals("on"))
            {
                throw new ParserException();
            }

            token = AssertNotEof(iterator.FirstOrDefault());

            if (!(token is NameToken && token.Value.Equals("on")))
            {
                throw new ParserException();
            }

            token = AssertNotEof(iterator.FirstOrDefault());

            if (!(token is NameToken))
            {
                throw new ParserException();
            }

            var type = token.Value;

            token = AssertNotEof(iterator.FirstOrDefault());

            var directives = ParseDirectives(ref token, iterator);

            if (!(token is PunctuatorToken && token.Value.Equals("{")))
            {
                throw new ParserException();
            }

            token = AssertNotEof(iterator.FirstOrDefault());

            var selectionSet = ParseSelectionSet(ref token, iterator);

            return new FragmentDefinition(name, type, directives, selectionSet);
        }

        private static IEnumerable<VariableDefinition> ParseVariableDefinitions(ListIterator<Token> iterator)
        {
            var token = AssertNotEof(iterator.FirstOrDefault());

            for (; ; )
            {
                if (token is PunctuatorToken && token.Value.Equals(")"))
                {
                    break;
                }

                if (!(token is PunctuatorToken && token.Value.Equals("$")))
                {
                    throw new ParserException();
                }

                var location = token.Location;

                token = AssertNotEof(iterator.FirstOrDefault());

                if (!(token is NameToken))
                {
                    throw new ParserException();
                }

                var name = token.Value;

                token = AssertNotEof(iterator.FirstOrDefault());

                if (!(token is PunctuatorToken && token.Value.Equals(":")))
                {
                    throw new ParserException();
                }

                token = AssertNotEof(iterator.FirstOrDefault());

                var type = ParseInputType(ref token, iterator);

                Value defaultValue = null;

                if (token is PunctuatorToken && token.Value.Equals("="))
                {
                    token = AssertNotEof(iterator.FirstOrDefault());

                    defaultValue = ParseValue(ref token, iterator);
                }

                yield return new VariableDefinition(name, type, defaultValue, location);
            }
        }

        private static InputType ParseInputType(ref Token token, ListIterator<Token> iterator)
        {
            if (token is PunctuatorToken && token.Value.Equals("["))
            {
                token = AssertNotEof(iterator.FirstOrDefault());

                var inputType = ParseInputType(ref token, iterator);

                // token is currently ']'
                token = AssertNotEof(iterator.FirstOrDefault());

                var listType = new ListType(inputType);
                InputType result = listType;

                if (token is PunctuatorToken && token.Value.Equals("!"))
                {
                    token = AssertNotEof(iterator.FirstOrDefault());

                    result = new NonNullType(listType);
                }

                return result;
            }

            if (!(token is NameToken))
            {
                throw new ParserException();
            }

            var namedType = new NamedType(token.Value);

            token = AssertNotEof(iterator.FirstOrDefault());

            if (token is PunctuatorToken && token.Value.Equals("!"))
            {
                token = AssertNotEof(iterator.FirstOrDefault());

                return new NonNullType(namedType);
            }

            return namedType;
        }

        private static IEnumerable<Directive> ParseDirectives(ref Token token, ListIterator<Token> iterator)
        {
            var result = new List<Directive>();

            for (; token is PunctuatorToken && token.Value.Equals("@");)
            {
                token = AssertNotEof(iterator.FirstOrDefault());

                if (!(token is NameToken))
                {
                    throw new ParserException();
                }

                var name = token.Value;

                token = AssertNotEof(iterator.FirstOrDefault());

                if (token is PunctuatorToken && token.Value.Equals("("))
                {
                    result.Add(new Directive(name, ParseArguments(iterator)));
                }
                else
                {
                    result.Add(new Directive(name, null));
                }
            }

            return result.Count == 0 ? null : result;
        }

        private static IEnumerable<Argument> ParseArguments(ListIterator<Token> iterator)
        {
            for (; ; )
            {
                var token = AssertNotEof(iterator.FirstOrDefault());

                if (token is PunctuatorToken && token.Value.Equals(")"))
                {
                    yield break;
                }
                else if (!(token is NameToken))
                {
                    throw new ParserException();
                }

                var (location, name) = token;

                token = AssertNotEof(iterator.FirstOrDefault());

                if (!(token is PunctuatorToken && token.Value.Equals(":")))
                {
                    throw new ParserException();
                }

                token = AssertNotEof(iterator.FirstOrDefault());

                var value = ParseValue(ref token, iterator);

                yield return new Argument(name, location, value);
            }
        }

        private static Value ParseValue(ref Token token, ListIterator<Token> iterator)
        {
            var result = TryParseValue(ref token, iterator);

            if (result == null)
            {
                throw new ParserException();
            }

            return result;
        }

        private static Value TryParseValue(ref Token token, ListIterator<Token> iterator)
        {
            if (token is PunctuatorToken)
            {
                if (token.Value.Equals("$"))
                {
                    var location = token.Location;

                    token = AssertNotEof(iterator.FirstOrDefault());

                    if (!(token is NameToken))
                    {
                        throw new ParserException();
                    }

                    return new Variable(token.Value, location);
                }

                if (token.Value.Equals("{"))
                {
                    return ParseObjectValue(ref token, iterator);
                }

                if (token.Value.Equals("["))
                {
                    return ParseListValue(ref token, iterator);
                }

                throw new ParserException();
            }

            if (token is IntValueToken intValueToken)
            {
                return new IntValue(intValueToken.IntValue);
            }

            if (token is FloatValueToken floatValueToken)
            {
                return new FloatValue(floatValueToken.DecimalValue);
            }

            if (token is StringValueToken)
            {
                return new StringValue(token.Value);
            }

            if (token is NameToken)
            {
                if (token.Value.Equals("true"))
                {
                    return new BooleanValue(true);
                }

                if (token.Value.Equals("false"))
                {
                    return new BooleanValue(false);
                }

                if (token.Value.Equals("null"))
                {
                    return new NullValue();
                }

                return new EnumValue(token.Value);
            }

            throw new ParserException();
        }

        private static ObjectValue ParseObjectValue(ref Token token, ListIterator<Token> iterator)
        {
            var fields = new List<ObjectField>();

            for (; ; )
            {
                token = AssertNotEof(iterator.FirstOrDefault());

                if (token is PunctuatorToken && token.Value.Equals("}"))
                {
                    break;
                }

                fields.Add(ParseObjectField(ref token, iterator));
            }

            return new ObjectValue(fields);
        }

        private static ListValue ParseListValue(ref Token token, ListIterator<Token> iterator)
        {
            var items = new List<Value>();

            for (; ; )
            {
                token = AssertNotEof(iterator.FirstOrDefault());

                if (token is PunctuatorToken && token.Value.Equals("]"))
                {
                    break;
                }

                items.Add(ParseValue(ref token, iterator));
            }

            return new ListValue(items);
        }

        private static ObjectField ParseObjectField(ref Token token, ListIterator<Token> iterator)
        {
            if (!(token is NameToken))
            {
                throw new ParserException();
            }

            var name = token.Value;

            token = AssertNotEof(iterator.FirstOrDefault());

            if (!(token is PunctuatorToken && token.Value.Equals(":")))
            {
                throw new ParserException();
            }

            token = AssertNotEof(iterator.FirstOrDefault());

            var value = ParseValue(ref token, iterator);

            return new ObjectField(name, value);
        }

        private static IEnumerable<Selection> ParseSelectionSet(ref Token token, ListIterator<Token> iterator)
        {
            var result = new List<Selection>();

            for (; ; )
            {
                if (token is PunctuatorToken)
                {
                    if (token.Value.Equals("}"))
                    {
                        break;
                    }

                    if (token.Value.Equals("..."))
                    {
                        result.Add(ParseFragmentSelection(ref token, iterator));
                    }
                    else
                    {
                        throw new ParserException();
                    }
                }

                if (token is NameToken)
                {
                    result.Add(ParseField(ref token, iterator));
                }
                else
                {
                    throw new ParserException();
                }
            }

            return result;
        }

        private static Field ParseField(ref Token token, ListIterator<Token> iterator)
        {
            string alias = null;
            var (location, name) = token;

            token = AssertNotEof(iterator.FirstOrDefault());

            if (token is PunctuatorToken && token.Value.Equals(":"))
            {
                token = AssertNotEof(iterator.FirstOrDefault());

                if (!(token is NameToken))
                {
                    throw new ParserException();
                }

                alias = name;
                name = token.Value;

                token = AssertNotEof(iterator.FirstOrDefault());
            }

            IEnumerable<Argument> arguments = null;

            if (token is PunctuatorToken && token.Value.Equals("("))
            {
                arguments = ParseArguments(iterator)
                    .ToList();

                token = AssertNotEof(iterator.FirstOrDefault());
            }

            IEnumerable<Directive> directives = null;

            if (token is PunctuatorToken && token.Value.Equals("@"))
            {
                directives = ParseDirectives(ref token, iterator);
            }

            IEnumerable<Selection> selectionSet = null;

            if (token is PunctuatorToken && token.Value.Equals("{"))
            {
                token = AssertNotEof(iterator.FirstOrDefault());

                selectionSet = ParseSelectionSet(ref token, iterator);

                token = AssertNotEof(iterator.FirstOrDefault());
            }

            return new Field(name, alias, arguments, directives, selectionSet, location);
        }

        private static Selection ParseFragmentSelection(ref Token token, ListIterator<Token> iterator)
        {
            token = AssertNotEof(iterator.FirstOrDefault());

            if (token is NameToken)
            {
                if (token.Value.Equals("on"))
                {
                    return ParseInlineFragment(ref token, iterator);
                }

                return ParseFragmentSpread(ref token, iterator);
            }

            return ParseInlineFragment(ref token, iterator);
        }

        private static FragmentSpread ParseFragmentSpread(ref Token token, ListIterator<Token> iterator)
        {
            var name = token.Value;

            token = AssertNotEof(iterator.FirstOrDefault());

            IEnumerable<Directive> directives = null;

            if (token is PunctuatorToken && token.Value.Equals("@"))
            {
                directives = ParseDirectives(ref token, iterator);
            }

            return new FragmentSpread(name, directives);
        }

        private static InlineFragment ParseInlineFragment(ref Token token, ListIterator<Token> iterator)
        {
            string type = null;

            if (token is NameToken)
            {
                if (!token.Value.Equals("on"))
                {
                    throw new ParserException();
                }

                token = AssertNotEof(iterator.FirstOrDefault());

                if (!(token is NameToken))
                {
                    throw new ParserException();
                }

                type = token.Value;

                token = AssertNotEof(iterator.FirstOrDefault());
            }

            if (!(token is PunctuatorToken))
            {
                throw new ParserException();
            }

            IEnumerable<Directive> directives = null;

            if (token.Value.Equals("@"))
            {
                directives = ParseDirectives(ref token, iterator);
            }

            if (!token.Value.Equals("{"))
            {
                throw new ParserException();
            }

            token = AssertNotEof(iterator.FirstOrDefault());

            var selectionSet = ParseSelectionSet(ref token, iterator);

            return new InlineFragment(type, directives, selectionSet);
        }

        private static Token AssertNotEof(Token value)
        {
            if (value == null)
            {
                throw new ParserException("EOF");
            }

            return value;
        }

        private sealed class ListIterator<T> : IEnumerable<T>
        {
            private readonly IList<T> source;
            private int index;

            public ListIterator(IList<T> source)
            {
                Check.IsNotNull(source, nameof(source));

                this.source = source;
            }

            public IEnumerator<T> GetEnumerator()
            {
                for (; index < source.Count;)
                {
                    yield return source[index++];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
