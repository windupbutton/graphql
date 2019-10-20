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
using WindupButton.GraphQL.Data;
using WindupButton.GraphQL.Schema;
using WindupButton.Roscoe.Expressions;

namespace WindupButton.GraphQL.Roscoe.SqlServer
{
    /// <summary>
    /// Paving over C# type system clumsiness :(
    /// </summary>
    public static class JsonQueryBuilderExtensions
    {
        public static IValueAccessor<GraphQLNotNull<GraphQLID>> Id(this JsonQueryBuilder builder, IDbFragment<Guid> value)
        {
            return builder.Select<GraphQLID, string>(new TypeCastDbValue<string>(value));
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLString>> String(this JsonQueryBuilder builder, IDbFragment<string> value)
        {
            return builder.Select<GraphQLString, string>(value);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLEnum<TEnum>>> Enum<TEnum>(this JsonQueryBuilder builder, IDbFragment<TEnum> value)
            where TEnum : struct, Enum
        {
            return builder.Select<GraphQLEnum<TEnum>, TEnum?>(new TypeCastDbValue<TEnum?>(value));
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLFloat>> Float(this JsonQueryBuilder builder, IDbFragment<decimal> value)
        {
            return builder.Select<GraphQLFloat, decimal?>(new TypeCastDbValue<decimal?>(value));
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLInt>> Int(this JsonQueryBuilder builder, IDbFragment<int> value)
        {
            return builder.Select<GraphQLInt, int?>(new TypeCastDbValue<int?>(value));
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLBoolean>> Bool(this JsonQueryBuilder builder, IDbFragment<bool> value)
        {
            return builder.Select<GraphQLBoolean, bool?>(new TypeCastDbValue<bool?>(value));
        }

        // ---

        public static IValueAccessor<GraphQLNotNull<GraphQLString>> String<T1>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            Func<T1, string> transformer)
        {
            return builder.Select<GraphQLString, T1, string>(value1, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLFloat>> Float<T1>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            Func<T1, decimal?> transformer)
        {
            return builder.Select<GraphQLFloat, T1, decimal?>(value1, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLInt>> Int<T1>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            Func<T1, int?> transformer)
        {
            return builder.Select<GraphQLInt, T1, int?>(value1, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLBoolean>> Bool<T1>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            Func<T1, bool?> transformer)
        {
            return builder.Select<GraphQLBoolean, T1, bool?>(value1, transformer);
        }

        // ---

        public static IValueAccessor<GraphQLNotNull<GraphQLString>> String<T1, T2>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            Func<T1, T2, string> transformer)
        {
            return builder.Select<GraphQLString, T1, T2, string>(value1, value2, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLFloat>> Float<T1, T2>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            Func<T1, T2, decimal?> transformer)
        {
            return builder.Select<GraphQLFloat, T1, T2, decimal?>(value1, value2, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLInt>> Int<T1, T2>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            Func<T1, T2, int?> transformer)
        {
            return builder.Select<GraphQLInt, T1, T2, int?>(value1, value2, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLBoolean>> Bool<T1, T2>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            Func<T1, T2, bool?> transformer)
        {
            return builder.Select<GraphQLBoolean, T1, T2, bool?>(value1, value2, transformer);
        }

        // ---

        public static IValueAccessor<GraphQLNotNull<GraphQLString>> String<T1, T2, T3>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            Func<T1, T2, T3, string> transformer)
        {
            return builder.Select<GraphQLString, T1, T2, T3, string>(value1, value2, value3, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLFloat>> Float<T1, T2, T3>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            Func<T1, T2, T3, decimal?> transformer)
        {
            return builder.Select<GraphQLFloat, T1, T2, T3, decimal?>(value1, value2, value3, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLInt>> Int<T1, T2, T3>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            Func<T1, T2, T3, int?> transformer)
        {
            return builder.Select<GraphQLInt, T1, T2, T3, int?>(value1, value2, value3, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLBoolean>> Bool<T1, T2, T3>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            Func<T1, T2, T3, bool?> transformer)
        {
            return builder.Select<GraphQLBoolean, T1, T2, T3, bool?>(value1, value2, value3, transformer);
        }

        // ---

        public static IValueAccessor<GraphQLNotNull<GraphQLString>> String<T1, T2, T3, T4>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            Func<T1, T2, T3, T4, string> transformer)
        {
            return builder.Select<GraphQLString, T1, T2, T3, T4, string>(value1, value2, value3, value4, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLFloat>> Float<T1, T2, T3, T4>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            Func<T1, T2, T3, T4, decimal?> transformer)
        {
            return builder.Select<GraphQLFloat, T1, T2, T3, T4, decimal?>(value1, value2, value3, value4, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLInt>> Int<T1, T2, T3, T4>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            Func<T1, T2, T3, T4, int?> transformer)
        {
            return builder.Select<GraphQLInt, T1, T2, T3, T4, int?>(value1, value2, value3, value4, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLBoolean>> Bool<T1, T2, T3, T4>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            Func<T1, T2, T3, T4, bool?> transformer)
        {
            return builder.Select<GraphQLBoolean, T1, T2, T3, T4, bool?>(value1, value2, value3, value4, transformer);
        }

        // ---

        public static IValueAccessor<GraphQLNotNull<GraphQLString>> String<T1, T2, T3, T4, T5>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            Func<T1, T2, T3, T4, T5, string> transformer)
        {
            return builder.Select<GraphQLString, T1, T2, T3, T4, T5, string>(value1, value2, value3, value4, value5, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLFloat>> Float<T1, T2, T3, T4, T5>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            Func<T1, T2, T3, T4, T5, decimal?> transformer)
        {
            return builder.Select<GraphQLFloat, T1, T2, T3, T4, T5, decimal?>(value1, value2, value3, value4, value5, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLInt>> Int<T1, T2, T3, T4, T5>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            Func<T1, T2, T3, T4, T5, int?> transformer)
        {
            return builder.Select<GraphQLInt, T1, T2, T3, T4, T5, int?>(value1, value2, value3, value4, value5, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLBoolean>> Bool<T1, T2, T3, T4, T5>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            Func<T1, T2, T3, T4, T5, bool?> transformer)
        {
            return builder.Select<GraphQLBoolean, T1, T2, T3, T4, T5, bool?>(value1, value2, value3, value4, value5, transformer);
        }

        // ---

        public static IValueAccessor<GraphQLNotNull<GraphQLString>> String<T1, T2, T3, T4, T5, T6>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            IDbFragment<T6> value6,
            Func<T1, T2, T3, T4, T5, T6, string> transformer)
        {
            return builder.Select<GraphQLString, T1, T2, T3, T4, T5, T6, string>(value1, value2, value3, value4, value5, value6, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLFloat>> Float<T1, T2, T3, T4, T5, T6>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            IDbFragment<T6> value6,
            Func<T1, T2, T3, T4, T5, T6, decimal?> transformer)
        {
            return builder.Select<GraphQLFloat, T1, T2, T3, T4, T5, T6, decimal?>(value1, value2, value3, value4, value5, value6, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLInt>> Int<T1, T2, T3, T4, T5, T6>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            IDbFragment<T6> value6,
            Func<T1, T2, T3, T4, T5, T6, int?> transformer)
        {
            return builder.Select<GraphQLInt, T1, T2, T3, T4, T5, T6, int?>(value1, value2, value3, value4, value5, value6, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLBoolean>> Bool<T1, T2, T3, T4, T5, T6>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            IDbFragment<T6> value6,
            Func<T1, T2, T3, T4, T5, T6, bool?> transformer)
        {
            return builder.Select<GraphQLBoolean, T1, T2, T3, T4, T5, T6, bool?>(value1, value2, value3, value4, value5, value6, transformer);
        }

        // ---

        public static IValueAccessor<GraphQLNotNull<GraphQLString>> String<T1, T2, T3, T4, T5, T6, T7>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            IDbFragment<T6> value6,
            IDbFragment<T7> value7,
            Func<T1, T2, T3, T4, T5, T6, T7, string> transformer)
        {
            return builder.Select<GraphQLString, T1, T2, T3, T4, T5, T6, T7, string>(value1, value2, value3, value4, value5, value6, value7, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLFloat>> Float<T1, T2, T3, T4, T5, T6, T7>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            IDbFragment<T6> value6,
            IDbFragment<T7> value7,
            Func<T1, T2, T3, T4, T5, T6, T7, decimal?> transformer)
        {
            return builder.Select<GraphQLFloat, T1, T2, T3, T4, T5, T6, T7, decimal?>(value1, value2, value3, value4, value5, value6, value7, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLInt>> Int<T1, T2, T3, T4, T5, T6, T7>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            IDbFragment<T6> value6,
            IDbFragment<T7> value7,
            Func<T1, T2, T3, T4, T5, T6, T7, int?> transformer)
        {
            return builder.Select<GraphQLInt, T1, T2, T3, T4, T5, T6, T7, int?>(value1, value2, value3, value4, value5, value6, value7, transformer);
        }

        public static IValueAccessor<GraphQLNotNull<GraphQLBoolean>> Bool<T1, T2, T3, T4, T5, T6, T7>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            IDbFragment<T6> value6,
            IDbFragment<T7> value7,
            Func<T1, T2, T3, T4, T5, T6, T7, bool?> transformer)
        {
            return builder.Select<GraphQLBoolean, T1, T2, T3, T4, T5, T6, T7, bool?>(value1, value2, value3, value4, value5, value6, value7, transformer);
        }

        // ---

        public static IValueAccessor<GraphQLString> NullableString(this JsonQueryBuilder builder, IDbFragment<string> value)
        {
            return builder.SelectNullable<GraphQLString, string>(value);
        }

        public static IValueAccessor<GraphQLEnum<TEnum>> NullableEnum<TEnum>(this JsonQueryBuilder builder, IDbFragment<TEnum> value)
            where TEnum : struct, Enum
        {
            return builder.SelectNullable<GraphQLEnum<TEnum>, TEnum?>(new TypeCastDbValue<TEnum?>(value));
        }

        public static IValueAccessor<GraphQLFloat> NullableFloat(this JsonQueryBuilder builder, IDbFragment<decimal> value)
        {
            return builder.SelectNullable<GraphQLFloat, decimal?>(new TypeCastDbValue<decimal?>(value));
        }

        public static IValueAccessor<GraphQLInt> NullableInt(this JsonQueryBuilder builder, IDbFragment<int> value)
        {
            return builder.SelectNullable<GraphQLInt, int?>(new TypeCastDbValue<int?>(value));
        }

        public static IValueAccessor<GraphQLBoolean> NullableBool(this JsonQueryBuilder builder, IDbFragment<bool> value)
        {
            return builder.SelectNullable<GraphQLBoolean, bool?>(new TypeCastDbValue<bool?>(value));
        }

        // ---

        public static IValueAccessor<GraphQLString> NullableString<T1>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            Func<T1, string> transformer)
        {
            return builder.SelectNullable<GraphQLString, T1, string>(value1, transformer);
        }

        public static IValueAccessor<GraphQLFloat> NullableFloat<T1>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            Func<T1, decimal?> transformer)
        {
            return builder.SelectNullable<GraphQLFloat, T1, decimal?>(value1, transformer);
        }

        public static IValueAccessor<GraphQLInt> NullableInt<T1>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            Func<T1, int?> transformer)
        {
            return builder.SelectNullable<GraphQLInt, T1, int?>(value1, transformer);
        }

        public static IValueAccessor<GraphQLBoolean> NullableBool<T1>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            Func<T1, bool?> transformer)
        {
            return builder.SelectNullable<GraphQLBoolean, T1, bool?>(value1, transformer);
        }

        // ---

        public static IValueAccessor<GraphQLString> NullableString<T1, T2>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            Func<T1, T2, string> transformer)
        {
            return builder.SelectNullable<GraphQLString, T1, T2, string>(value1, value2, transformer);
        }

        public static IValueAccessor<GraphQLFloat> NullableFloat<T1, T2>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            Func<T1, T2, decimal?> transformer)
        {
            return builder.SelectNullable<GraphQLFloat, T1, T2, decimal?>(value1, value2, transformer);
        }

        public static IValueAccessor<GraphQLInt> NullableInt<T1, T2>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            Func<T1, T2, int?> transformer)
        {
            return builder.SelectNullable<GraphQLInt, T1, T2, int?>(value1, value2, transformer);
        }

        public static IValueAccessor<GraphQLBoolean> NullableBool<T1, T2>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            Func<T1, T2, bool?> transformer)
        {
            return builder.SelectNullable<GraphQLBoolean, T1, T2, bool?>(value1, value2, transformer);
        }

        // ---

        public static IValueAccessor<GraphQLString> NullableString<T1, T2, T3>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            Func<T1, T2, T3, string> transformer)
        {
            return builder.SelectNullable<GraphQLString, T1, T2, T3, string>(value1, value2, value3, transformer);
        }

        public static IValueAccessor<GraphQLFloat> NullableFloat<T1, T2, T3>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            Func<T1, T2, T3, decimal?> transformer)
        {
            return builder.SelectNullable<GraphQLFloat, T1, T2, T3, decimal?>(value1, value2, value3, transformer);
        }

        public static IValueAccessor<GraphQLInt> NullableInt<T1, T2, T3>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            Func<T1, T2, T3, int?> transformer)
        {
            return builder.SelectNullable<GraphQLInt, T1, T2, T3, int?>(value1, value2, value3, transformer);
        }

        public static IValueAccessor<GraphQLBoolean> NullableBool<T1, T2, T3>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            Func<T1, T2, T3, bool?> transformer)
        {
            return builder.SelectNullable<GraphQLBoolean, T1, T2, T3, bool?>(value1, value2, value3, transformer);
        }

        // ---

        public static IValueAccessor<GraphQLString> NullableString<T1, T2, T3, T4>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            Func<T1, T2, T3, T4, string> transformer)
        {
            return builder.SelectNullable<GraphQLString, T1, T2, T3, T4, string>(value1, value2, value3, value4, transformer);
        }

        public static IValueAccessor<GraphQLFloat> NullableFloat<T1, T2, T3, T4>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            Func<T1, T2, T3, T4, decimal?> transformer)
        {
            return builder.SelectNullable<GraphQLFloat, T1, T2, T3, T4, decimal?>(value1, value2, value3, value4, transformer);
        }

        public static IValueAccessor<GraphQLInt> NullableInt<T1, T2, T3, T4>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            Func<T1, T2, T3, T4, int?> transformer)
        {
            return builder.SelectNullable<GraphQLInt, T1, T2, T3, T4, int?>(value1, value2, value3, value4, transformer);
        }

        public static IValueAccessor<GraphQLBoolean> NullableBool<T1, T2, T3, T4>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            Func<T1, T2, T3, T4, bool?> transformer)
        {
            return builder.SelectNullable<GraphQLBoolean, T1, T2, T3, T4, bool?>(value1, value2, value3, value4, transformer);
        }

        // ---

        public static IValueAccessor<GraphQLString> NullableString<T1, T2, T3, T4, T5>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            Func<T1, T2, T3, T4, T5, string> transformer)
        {
            return builder.SelectNullable<GraphQLString, T1, T2, T3, T4, T5, string>(value1, value2, value3, value4, value5, transformer);
        }

        public static IValueAccessor<GraphQLFloat> NullableFloat<T1, T2, T3, T4, T5>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            Func<T1, T2, T3, T4, T5, decimal?> transformer)
        {
            return builder.SelectNullable<GraphQLFloat, T1, T2, T3, T4, T5, decimal?>(value1, value2, value3, value4, value5, transformer);
        }

        public static IValueAccessor<GraphQLInt> NullableInt<T1, T2, T3, T4, T5>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            Func<T1, T2, T3, T4, T5, int?> transformer)
        {
            return builder.SelectNullable<GraphQLInt, T1, T2, T3, T4, T5, int?>(value1, value2, value3, value4, value5, transformer);
        }

        public static IValueAccessor<GraphQLBoolean> NullableBool<T1, T2, T3, T4, T5>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            Func<T1, T2, T3, T4, T5, bool?> transformer)
        {
            return builder.SelectNullable<GraphQLBoolean, T1, T2, T3, T4, T5, bool?>(value1, value2, value3, value4, value5, transformer);
        }

        // ---

        public static IValueAccessor<GraphQLString> NullableString<T1, T2, T3, T4, T5, T6>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            IDbFragment<T6> value6,
            Func<T1, T2, T3, T4, T5, T6, string> transformer)
        {
            return builder.SelectNullable<GraphQLString, T1, T2, T3, T4, T5, T6, string>(value1, value2, value3, value4, value5, value6, transformer);
        }

        public static IValueAccessor<GraphQLFloat> NullableFloat<T1, T2, T3, T4, T5, T6>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            IDbFragment<T6> value6,
            Func<T1, T2, T3, T4, T5, T6, decimal?> transformer)
        {
            return builder.SelectNullable<GraphQLFloat, T1, T2, T3, T4, T5, T6, decimal?>(value1, value2, value3, value4, value5, value6, transformer);
        }

        public static IValueAccessor<GraphQLInt> NullableInt<T1, T2, T3, T4, T5, T6>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            IDbFragment<T6> value6,
            Func<T1, T2, T3, T4, T5, T6, int?> transformer)
        {
            return builder.SelectNullable<GraphQLInt, T1, T2, T3, T4, T5, T6, int?>(value1, value2, value3, value4, value5, value6, transformer);
        }

        public static IValueAccessor<GraphQLBoolean> NullableBool<T1, T2, T3, T4, T5, T6>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            IDbFragment<T6> value6,
            Func<T1, T2, T3, T4, T5, T6, bool?> transformer)
        {
            return builder.SelectNullable<GraphQLBoolean, T1, T2, T3, T4, T5, T6, bool?>(value1, value2, value3, value4, value5, value6, transformer);
        }

        // ---

        public static IValueAccessor<GraphQLString> NullableString<T1, T2, T3, T4, T5, T6, T7>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            IDbFragment<T6> value6,
            IDbFragment<T7> value7,
            Func<T1, T2, T3, T4, T5, T6, T7, string> transformer)
        {
            return builder.SelectNullable<GraphQLString, T1, T2, T3, T4, T5, T6, T7, string>(value1, value2, value3, value4, value5, value6, value7, transformer);
        }

        public static IValueAccessor<GraphQLFloat> NullableFloat<T1, T2, T3, T4, T5, T6, T7>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            IDbFragment<T6> value6,
            IDbFragment<T7> value7,
            Func<T1, T2, T3, T4, T5, T6, T7, decimal?> transformer)
        {
            return builder.SelectNullable<GraphQLFloat, T1, T2, T3, T4, T5, T6, T7, decimal?>(value1, value2, value3, value4, value5, value6, value7, transformer);
        }

        public static IValueAccessor<GraphQLInt> NullableInt<T1, T2, T3, T4, T5, T6, T7>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            IDbFragment<T6> value6,
            IDbFragment<T7> value7,
            Func<T1, T2, T3, T4, T5, T6, T7, int?> transformer)
        {
            return builder.SelectNullable<GraphQLInt, T1, T2, T3, T4, T5, T6, T7, int?>(value1, value2, value3, value4, value5, value6, value7, transformer);
        }

        public static IValueAccessor<GraphQLBoolean> NullableBool<T1, T2, T3, T4, T5, T6, T7>(
            this JsonQueryBuilder builder,
            IDbFragment<T1> value1,
            IDbFragment<T2> value2,
            IDbFragment<T3> value3,
            IDbFragment<T4> value4,
            IDbFragment<T5> value5,
            IDbFragment<T6> value6,
            IDbFragment<T7> value7,
            Func<T1, T2, T3, T4, T5, T6, T7, bool?> transformer)
        {
            return builder.SelectNullable<GraphQLBoolean, T1, T2, T3, T4, T5, T6, T7, bool?>(value1, value2, value3, value4, value5, value6, value7, transformer);
        }
    }
}
