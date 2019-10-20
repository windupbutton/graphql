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
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WindupButton.GraphQL.Ast;
using WindupButton.GraphQL.Data;
using WindupButton.GraphQL.Schema;

namespace WindupButton.GraphQL.Execution
{
    internal static class FieldCollector
    {
        // todo: directives
        public static async Task<IEnumerable<FieldSelection>> CollectFields(
            VariableValues variableValues,
            IDictionary<string, InputType> variableTypes,
            GraphQLObject gqlObject,
            IEnumerable<Selection> selectionSet,
            IEnumerable<FragmentDefinition> fragments,
            List<GraphQLError> errors,
            ILogger logger,
            ExceptionFilter exceptionFilter,
            CancellationToken token)
        {
            var result = new List<FieldSelection>();

            foreach (var selection in selectionSet)
            {
                switch (selection)
                {
                    case Ast.Field field:
                        {
                            IValueAccessor valueAccessor;
                            var selectionFieldSelectionSet = Enumerable.Empty<FieldSelection>();

                            if (!gqlObject.Fields.TryGetValue(field.Name, out var objectField))
                            {
                                valueAccessor = new NullValueAccessor();

                                errors.Add(new GraphQLError(
                                    $"Invalid field '{field.Name}' specified on '{gqlObject.GetTypeName()}'.",
                                    new[] { field.Location }));
                            }
                            else
                            {
                                var fieldArguments = field.Arguments ?? Enumerable.Empty<Argument>();

                                if (!ValidateArguments(variableTypes, gqlObject, errors, objectField, fieldArguments))
                                {
                                    valueAccessor = new NullValueAccessor();
                                }
                                else
                                {
                                    var astFieldArguments = fieldArguments
                                        .ToDictionary(x => x.Name);
                                    var argumentValues = new Dictionary<string, object>();
                                    var hasErrors = false;

                                    foreach (var argument in objectField.Arguments)
                                    {
                                        var hasArgument = astFieldArguments.TryGetValue(argument.Key, out var argumentValue);
                                        var value = hasArgument
                                            ? argumentValue.Value.GetValue(variableValues)
                                            : new Optional();
                                        var variable = argumentValue?.Value as Variable;

                                        var preamble = variable != null
                                            ? $"Variable '${variable.Name}'"
                                            : $"Argument '{argument.Key}'";

                                        var locations = new List<GraphQLLocation>();

                                        if (variable != null)
                                        {
                                            locations.Add(variable.Location);
                                        }
                                        if (hasArgument)
                                        {
                                            locations.Add(argumentValue.Location);
                                        }

                                        // need to be able to get a type adapter from an AST node

                                        //var target = variable != null
                                        //    ? new BoundGraphQLTypeAdapter(null, variableTypes[variable.Name].GetTypeAdapter())
                                        //    : argument.Value;

                                        var target = argument.Value;

                                        var coercionResult = target.CoerceInput(argument.Key, value.Value);

                                        if (coercionResult.Errors != null && coercionResult.Errors.Any())
                                        {
                                            errors.AddRange(coercionResult.Errors
                                                .Select(x => new GraphQLError(
                                                    $"{preamble} expected value of type '{argument.Value.GetTypeDescription()}' but got: {value}. {x}",
                                                    locations,
                                                    null,
                                                    new Dictionary<string, object>
                                                    {
                                                        {
                                                            "InputError",
                                                            new
                                                            {
                                                                Input = x.Name,
                                                                Reason = "Coercion",
                                                            }
                                                        },
                                                    }))
                                                .ToList()
                                            );

                                            hasErrors = true;
                                        }
                                        else
                                        {
                                            argumentValues.Add(argument.Key, coercionResult.Value);
                                        }
                                    }

                                    if (hasErrors)
                                    {
                                        valueAccessor = new NullValueAccessor();
                                    }
                                    else
                                    {
                                        try
                                        {
                                            valueAccessor = await objectField.Resolve(argumentValues, token);

                                            selectionFieldSelectionSet = valueAccessor.GraphQLType.Unwrap() is GraphQLObject graphQLObject
                                                ? await CollectFields(
                                                    variableValues,
                                                    variableTypes,
                                                    graphQLObject,
                                                    field.SelectionSet,
                                                    fragments,
                                                    errors,
                                                    logger,
                                                    exceptionFilter,
                                                    token)
                                                : null;
                                        }
                                        catch (GraphQLException ex)
                                        {
                                            errors.AddRange(ex.Errors);

                                            valueAccessor = new NullValueAccessor();
                                        }
                                        catch (Exception ex)
                                        {
                                            if (ex is TargetInvocationException || ex is AggregateException)
                                            {
                                                ex = ex.InnerException ?? ex;
                                            }

                                            if (ex is GraphQLException gex)
                                            {
                                                errors.AddRange(gex.Errors);
                                            }
                                            else
                                            {
                                                if (exceptionFilter != null && await exceptionFilter.HandleExceptionAsync(ex))
                                                {
                                                    throw;
                                                }

                                                if (logger != null)
                                                {
                                                    logger.LogError(ex, "Error resolving field in FieldCollector");
                                                }

                                                errors.Add(new GraphQLError(
                                                    "Internal server error",
                                                    null,
                                                    null,
                                                    null));
                                            }

                                            valueAccessor = new NullValueAccessor();
                                        }
                                    }

                                    var fieldSelection = new FieldSelection(
                                        field.Name,
                                        field.Alias,
                                        field.Arguments,
                                        field.Directives,
                                        selectionFieldSelectionSet,
                                        objectField,
                                        field.Location,
                                        valueAccessor
                                    );

                                    var index = result
                                        .FindIndex(x => x.Alias == field.Alias && x.Name == field.Name);

                                    if (index >= 0)
                                    {
                                        result[index] = result[index].MergeWith(fieldSelection);
                                    }
                                    else
                                    {
                                        result.Add(fieldSelection);
                                    }
                                }
                            }
                        }

                        break;

                    case FragmentSpread fragmentSpread:
                        {
                            var fragment = fragments
                                .Where(x => x.FragmentName == fragmentSpread.FragmentName)
                                .FirstOrDefault() ?? throw new InvalidOperationException("Invalid fragment");

                            if (fragment.TypeCondition == gqlObject.GetTypeName())
                            {
                                var fields = await CollectFields(
                                    variableValues,
                                    variableTypes,
                                    gqlObject,
                                    fragment.SelectionSet,
                                    fragments,
                                    errors,
                                    logger,
                                    exceptionFilter,
                                    token
                                );

                                foreach (var field in fields)
                                {
                                    var index = result
                                        .FindIndex(x => x.Alias == field.Alias && x.Name == field.Name);

                                    if (index >= 0)
                                    {
                                        result[index] = result[index].MergeWith(field);
                                    }
                                    else
                                    {
                                        result.Add(field);
                                    }
                                }
                            }
                        }

                        break;

                    case InlineFragment inlineFragment:
                        {
                            if (inlineFragment.TypeCondition == gqlObject.GetTypeName())
                            {
                                var fields = await CollectFields(
                                    variableValues,
                                    variableTypes,
                                    gqlObject,
                                    inlineFragment.SelectionSet,
                                    fragments,
                                    errors,
                                    logger,
                                    exceptionFilter,
                                    token
                                );

                                foreach (var field in fields)
                                {
                                    var index = result
                                        .FindIndex(x => x.Alias == field.Alias && x.Name == field.Name);

                                    if (index >= 0)
                                    {
                                        result[index] = result[index].MergeWith(field);
                                    }
                                    else
                                    {
                                        result.Add(field);
                                    }
                                }
                            }
                        }

                        break;
                }
            }

            return result;
        }

        private static bool ValidateArguments(IDictionary<string, InputType> variableTypes, GraphQLObject gqlObject, List<GraphQLError> errors, Schema.Field objectField, IEnumerable<Argument> fieldArguments)
        {
            var valid = true;

            foreach (var arg in fieldArguments)
            {
                var inputType = arg.Value.GetType(variableTypes);

                if (!objectField.Arguments.TryGetValue(arg.Name, out var graphQLType))
                {
                    valid = false;

                    errors.Add(new GraphQLError(
                        $"Unknown argument '{arg.Name}' on field '{objectField.Name}' of type '{gqlObject.GetTypeName()}'.",
                        new[] { arg.Location }));
                }
                else if (arg.Value is Variable variable)
                {
                    var inputTypeDescription = inputType.GetTypeDescription();
                    var graphQLTypeDescription = graphQLType.GetTypeDescription();

                    if (!inputTypeDescription.CanBeDownCastTo(graphQLTypeDescription))
                    {
                        valid = false;

                        errors.Add(new GraphQLError(
                            $"Variable '${arg.Name}' of type '{inputTypeDescription}' used in position expecting type '{graphQLTypeDescription}'.",
                            new[] { variable.Location, arg.Location }));
                    }
                }

                // todo: validate non-variable values
            }

            return valid;
        }
    }
}
