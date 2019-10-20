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
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using WindupButton.GraphQL.Schema;

namespace WindupButton.GraphQL.Data
{
    public class SchemaExpressionVisitor : ExpressionVisitor
    {
        private readonly IGraphQLType parent;

        protected SchemaExpressionVisitor(IGraphQLType parent)
        {
            this.parent = parent;

            DataReferenceParameter = Expression.Parameter(typeof(DataReference));
        }

        protected ParameterExpression DataReferenceParameter { get; }

        public static Expression<Func<TObject, DataReference, T>> VisitValueAccessors<TObject, T>(GraphQLObject parent, Expression<Func<TObject, T>> expression)
            where TObject : GraphQLObject
        {
            var visitor = new SchemaExpressionVisitor(parent);
            var result = Expression.Lambda<Func<TObject, DataReference, T>>(expression.Body, expression.Parameters[0], visitor.DataReferenceParameter);

            return (Expression<Func<TObject, DataReference, T>>)visitor.Visit(result);
        }

        protected static Expression VisitValueAccessors(IGraphQLType parent, LambdaExpression expression)
        {
            var visitor = new SchemaExpressionVisitor(parent);
            var result = Expression.Lambda(expression.Body, expression.Parameters[0], visitor.DataReferenceParameter);

            return visitor.Visit(result);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(GraphQLSchemaExtensions)
                && node.Method.Name == nameof(GraphQLSchemaExtensions.Select))
            {
                var parameter = FindParameter(node.Arguments[0]);
                var childParent = (IValueAccessor)Expression.Lambda(node.Arguments[0], parameter).Compile().DynamicInvoke(parent);
                var unwrappedChildParent = Unwrap(childParent.GraphQLType);

                return Expression.Call(
                    GraphQLSchemaOperations.Methods[node.Method.GetCustomAttribute<DescriptionAttribute>().Description].MakeGenericMethod(node.Method.GetGenericArguments()),
                    //node.Arguments[0],
                    Expression.Constant(childParent),
                    DataReferenceParameter,
                    VisitValueAccessors(unwrappedChildParent, (LambdaExpression)node.Arguments[1]));
            }

            if (node.Method.DeclaringType == typeof(GraphQLSchemaExtensions)
                && node.Method.Name == nameof(GraphQLSchemaExtensions.Value))
            {
                var parameter = FindParameter(node.Arguments[0]);
                var lambda = Expression.Lambda(node.Arguments[0], parameter).Compile();
                var result = lambda.DynamicInvoke(parent);

                return Expression.Convert(
                    Expression.Call(
                        //node.Arguments[0],
                        Expression.Constant(result),
                        typeof(IValueAccessor).GetMethod(nameof(IValueAccessor.GetValue)),
                        DataReferenceParameter),
                    node.Method.ReturnType);
            }

            return base.VisitMethodCall(node);
        }

        private static IGraphQLType Unwrap(IGraphQLType gqlType)
        {
            var result = gqlType.Unwrap();
            for (var previous = gqlType; result != previous; result = result.Unwrap())
            {
                previous = result;
            }

            return result;
        }

        private static ParameterExpression FindParameter(Expression node)
        {
            for (var currentNode = node; ;)
            {
                if (currentNode is MemberExpression memberExpression)
                {
                    currentNode = memberExpression.Expression;
                }
                else if (currentNode is ParameterExpression parameterExpression)
                {
                    return parameterExpression;
                }
                if (currentNode is MethodCallExpression methodCallExpression)
                {
                    currentNode = methodCallExpression.Object;
                }
                else
                {
                    throw new Exception($"Could not find parameter expression for node {node}");
                }
            }
        }
    }
}
