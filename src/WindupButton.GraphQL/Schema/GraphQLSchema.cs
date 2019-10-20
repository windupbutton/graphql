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
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WindupButton.GraphQL.Ast;
using WindupButton.GraphQL.Data;
using WindupButton.GraphQL.Execution;
using WindupButton.GraphQL.Parsing;

namespace WindupButton.GraphQL.Schema
{
    public sealed class GraphQLSchema<TQuery, TMutation>
        where TQuery : GraphQLObject
        where TMutation : GraphQLObject
    {
        private readonly Func<IAsyncBatch, TQuery> queryFactory;
        private readonly Func<IAsyncBatch, TMutation> mutationFactory;

        private readonly ILogger logger;
        private readonly ExceptionFilter exceptionFilter;

        public GraphQLSchema(
            Func<IAsyncBatch, TQuery> queryFactory,
            Func<IAsyncBatch, TMutation> mutationFactory,
            ILoggerFactory loggerFactory = null,
            ExceptionFilter exceptionFilter = null)
        {
            Check.IsNotNull(queryFactory, nameof(queryFactory));

            this.queryFactory = queryFactory;
            this.mutationFactory = mutationFactory;
            logger = loggerFactory?.CreateLogger<GraphQLSchema<TQuery, TMutation>>();
            this.exceptionFilter = exceptionFilter;
        }

        public async Task<T> QueryAsync<T>(Expression<Func<TQuery, T>> selector, CancellationToken token = default)
        {
            Check.IsNotNull(selector, nameof(selector));

            var asyncBatch = new AsyncBatch(true);
            var query = queryFactory(asyncBatch);
            var resultFactory = SchemaExpressionVisitor.VisitValueAccessors(query, selector).Compile();

            await asyncBatch.ExecuteAsync(token);

            return resultFactory(query, null);
        }

        public async Task<T> MutateAsync<T>(Expression<Func<TMutation, T>> selector, CancellationToken token = default)
        {
            Check.IsNotNull(selector, nameof(selector));

            var asyncBatch = new AsyncBatch(true);
            var mutation = mutationFactory(asyncBatch);
            var resultFactory = SchemaExpressionVisitor.VisitValueAccessors(mutation, selector).Compile();

            await asyncBatch.ExecuteAsync(token);

            return resultFactory(mutation, null);
        }

        public async Task<OperationResult> ExecuteRequestAsync(
            string queryText,
            IDictionary<string, object> variableValues,
            string operationName = null,
            CancellationToken token = default)
        {
            Check.IsNotNullOrWhiteSpace(queryText, nameof(queryText));

            var document = new Parser().Parse(new Lexer().Tokenize(queryText));

            var operations = document.Definitions
                .OfType<OperationDefinition>();

            if (operationName != null)
            {
                operations = operations
                    .Where(x => x.Name == operationName);
            }

            var operationCount = operations
                .Take(2)
                .Count();

            if (operationCount == 0)
            {
                throw new InvalidOperationException("No operations defined");
            }
            else if (operationCount > 1)
            {
                throw new InvalidOperationException("Multiple operations defined");
            }

            var operation = operations
                .First();

            var fragments = document.Definitions
                .OfType<FragmentDefinition>()
                .ToList();

            if (operation.OperationType != OperationType.Query && mutationFactory == null)
            {
                throw new InvalidOperationException("No mutation defined");
            }

            var asyncBatch = new AsyncBatch(operation.OperationType == OperationType.Query);
            var target = operation.OperationType == OperationType.Query
                ? (GraphQLObject)queryFactory(asyncBatch)
                : mutationFactory(asyncBatch);

            var errors = new List<GraphQLError>();
            var variableDefinitions = (operation.VariableDefinitions ?? Enumerable.Empty<VariableDefinition>())
                .ToDictionary(x => x.Name, x => x);
            var variableTypes = (operation.VariableDefinitions ?? Enumerable.Empty<VariableDefinition>())
                .ToDictionary(x => x.Name, x => x.Type);
            var variables = new VariableValues((operation.VariableDefinitions ?? Enumerable.Empty<VariableDefinition>())
                .ToDictionary(x => x.Name, x => variableValues.TryGetValue(x.Name, out var value) ? value : x.DefaultValue));
            var fields = await FieldCollector.CollectFields(variables, variableTypes, target, operation.SelectionSet, fragments, errors, logger, exceptionFilter, token);

            errors.AddRange(variables.UnusedVariables
                .Select(x => new GraphQLError($"Variable '${x}' is not used. ", new[] { variableDefinitions[x].Location }))
                .ToList()
            );

            if (errors.Any())
            {
                return new OperationResult(null, errors);
            }

            try
            {
                await asyncBatch.ExecuteAsync(token);
            }
            catch (GraphQLException ex)
            {
                return new OperationResult(null, ex.Errors);
            }

            var result = Apply(null, fields, errors);

            return new OperationResult(result, errors);
        }

        private IDictionary<string, object> Apply(DataReference value, IEnumerable<FieldSelection> selectionSet, List<GraphQLError> errors)
        {
            var result = new Dictionary<string, object>();

            foreach (var selection in selectionSet)
            {
                var fieldValue = selection.ValueAccessor.GetValue(value);

                if (fieldValue != null && selection.SelectionSet != null)
                {
                    fieldValue = selection.Field.IsSingular
                        ? (object)Apply(new DataReference(fieldValue), selection.SelectionSet, errors)
                        : ((IEnumerable)fieldValue)
                            ?.OfType<object>()
                            ?.Select(x => Apply(new DataReference(x), selection.SelectionSet, errors))
                            ?.ToList();
                }

                var coercedValue = selection.ValueAccessor.GraphQLType.TypeAdapter.CoerceResult(
                    selection.ValueAccessor.GraphQLType.GetType(),
                    selection.Name,
                    fieldValue);
                errors.AddRange((coercedValue.Errors ?? Enumerable.Empty<CoercionError>())
                    .Select(x => new GraphQLError($"{x.Name} could not be fetched", new[] { selection.Location }))
                    .ToList());

                result.Add(selection.Alias ?? selection.Name, coercedValue.Value);
            }

            return result;
        }
    }

    public static class GraphQLSchema
    {
        public static GraphQLSchema<TQuery, TMutation> Create<TQuery, TMutation>(
            Func<IAsyncBatch, TQuery> query,
            Func<IAsyncBatch, TMutation> mutation,
            ILoggerFactory loggerFactory = null,
            ExceptionFilter exceptionFilter = null)
                where TQuery : GraphQLObject
                where TMutation : GraphQLObject
        {
            return new GraphQLSchema<TQuery, TMutation>(query, mutation, loggerFactory, exceptionFilter);
        }

        public static GraphQLSchema<TQuery, GraphQLObject> Create<TQuery>(
            Func<IAsyncBatch, TQuery> query,
            ILoggerFactory loggerFactory = null,
            ExceptionFilter exceptionFilter = null)
                where TQuery : GraphQLObject
        {
            return new GraphQLSchema<TQuery, GraphQLObject>(query, x => null, loggerFactory, exceptionFilter);
        }
    }

    public static class GraphQLSchemaExtensions
    {
        public const string SelectObject = "selectObject";
        public const string SelectNullObject = "selectNullObject";

        public const string SelectList = "selectList";
        public const string SelectNullList = "selectNullList";
        public const string SelectListOfNull = "selectListOfNull";
        public const string SelectNullListOfNull = "selectNullListOfNull";

        [Description(SelectList)]
        public static IEnumerable<T> Select<TObject, T>(this IValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<TObject>>>> target, Func<TObject, T> selector)
            where TObject : GraphQLObject
        {
            throw new InvalidOperationException();
        }

        [Description(SelectListOfNull)]
        public static IEnumerable<T> Select<TObject, T>(this IValueAccessor<GraphQLNotNull<GraphQLList<TObject>>> target, Func<TObject, T> selector)
            where TObject : GraphQLObject
        {
            throw new InvalidOperationException();
        }

        [Description(SelectNullList)]
        public static IEnumerable<T> Select<TObject, T>(this IValueAccessor<GraphQLList<GraphQLNotNull<TObject>>> target, Func<TObject, T> selector)
            where TObject : GraphQLObject
        {
            throw new InvalidOperationException();
        }

        [Description(SelectNullListOfNull)]
        public static IEnumerable<T> Select<TObject, T>(this IValueAccessor<GraphQLList<TObject>> target, Func<TObject, T> selector)
            where TObject : GraphQLObject
        {
            throw new InvalidOperationException();
        }

        [Description(SelectNullObject)]
        public static T Select<TObject, T>(this IValueAccessor<TObject> target, Func<TObject, T> selector)
            where TObject : GraphQLObject
        {
            throw new InvalidOperationException();
        }

        [Description(SelectObject)]
        public static T Select<TObject, T>(this IValueAccessor<GraphQLNotNull<TObject>> target, Func<TObject, T> selector)
            where TObject : GraphQLObject
        {
            throw new InvalidOperationException();
        }

        // ---

        public static T Value<T>(this IValueAccessor<IGraphQLNotNull<IGraphQLType<T?>>> value)
            where T : struct
        {
            throw new InvalidOperationException();
        }

        public static T Value<T>(this IValueAccessor<IGraphQLNotNull<IGraphQLType<T>>> value)
            where T : class
        {
            throw new InvalidOperationException();
        }

        public static T Value<T>(this IValueAccessor<IGraphQLType<T>> value)
        {
            throw new InvalidOperationException();
        }
    }
}
