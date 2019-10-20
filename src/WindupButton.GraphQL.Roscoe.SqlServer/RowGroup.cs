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

namespace WindupButton.GraphQL.Roscoe.SqlServer
{
    internal class RowGroup
    {
        private static readonly KeyEqualityComparer keyEqualityComparer = new KeyEqualityComparer();

        private readonly List<RowGroup> children;

        public RowGroup(string name, IEnumerable<string> key, bool isSingular)
        {
            Check.IsNotNull(name, nameof(name));
            Check.IsNotNull(key, nameof(key));

            Name = name;
            Key = key;
            IsSingular = isSingular;

            children = new List<RowGroup>();
        }

        public string Name { get; }
        public IEnumerable<string> Key { get; }
        public bool IsSingular { get; }

        public IEnumerable<RowGroup> Children => children;

        public void AddChild(RowGroup rowGroup)
        {
            children.Add(rowGroup);
        }

        public object Apply(IEnumerable<IDictionary<string, object>> data)
        {
            if (!data.Any())
            {
                var row = new Dictionary<string, object>();

                foreach (var child in Children)
                {
                    row.Add(child.Name, child.ApplyRecursive(data));
                }

                return row;
            }

            return ApplyRecursive(data);
        }

        private object ApplyRecursive(IEnumerable<IDictionary<string, object>> data)
        {
            var result = data
                .GroupBy(x => Key.Select(y => x[y]).ToArray(), keyEqualityComparer) // .ToArray is important - .ToList doesn't work correctly
                .Where(x => !x.Key.Any(y => y == null))
                .Select(x =>
                {
                    var parent = x.First();

                    foreach (var child in Children)
                    {
                        parent.Add(child.Name, child.ApplyRecursive(x));
                    }

                    return parent;
                })
                .ToList();

            if (IsSingular)
            {
                if (result.Count > 1)
                {
                    throw new InvalidOperationException();
                }

                return result.FirstOrDefault();
            }

            return result;
        }

        private sealed class KeyEqualityComparer : IEqualityComparer<object[]>
        {
            public bool Equals(object[] x, object[] y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                if (x == null && y != null || x != null && y == null)
                {
                    return false;
                }

                return x.SequenceEqual(y);
            }

            public int GetHashCode(object[] obj)
            {
                if (obj == null)
                {
                    return 0;
                }

                return obj.Aggregate(0, (x, y) => x ^ (y?.GetHashCode() ?? 0));
            }
        }
    }
}
