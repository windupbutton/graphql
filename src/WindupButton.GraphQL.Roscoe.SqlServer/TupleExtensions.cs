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

namespace WindupButton.GraphQL.Roscoe.SqlServer
{
    internal static class TupleExtensions
    {
        public static IEnumerable<T> ToArray<T>(this (T, T) value)
        {
            return new[] { value.Item1, value.Item2 };
        }

        public static IEnumerable<T> ToArray<T>(this (T, T, T) value)
        {
            return new[] { value.Item1, value.Item2, value.Item3 };
        }

        public static IEnumerable<T> ToArray<T>(this (T, T, T, T) value)
        {
            return new[] { value.Item1, value.Item2, value.Item3, value.Item4 };
        }

        public static IEnumerable<T> ToArray<T>(this (T, T, T, T, T) value)
        {
            return new[] { value.Item1, value.Item2, value.Item3, value.Item4, value.Item5 };
        }

        public static IEnumerable<T> ToArray<T>(this (T, T, T, T, T, T) value)
        {
            return new[] { value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6 };
        }

        public static IEnumerable<T> ToArray<T>(this (T, T, T, T, T, T, T) value)
        {
            return new[] { value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6, value.Item7 };
        }

        public static IEnumerable<T> ToArray<T>(this (T, T, T, T, T, T, T, T) value)
        {
            return new[] { value.Item1, value.Item2, value.Item3, value.Item4, value.Item5, value.Item6, value.Item7, value.Item8 };
        }
    }
}
