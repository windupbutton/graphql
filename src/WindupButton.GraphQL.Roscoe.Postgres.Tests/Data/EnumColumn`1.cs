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
using Microsoft.Extensions.DependencyInjection;
using WindupButton.Roscoe.Expressions;
using WindupButton.Roscoe.Infrastructure;
using WindupButton.Roscoe.Schema;

namespace WindupButton.GraphQL.Roscoe.Postgres.Tests.Data
{
    public class EnumColumn<T> : DbEnum<T>, IColumn
        where T : struct, Enum
    {
        private readonly IDictionary<string, object> properties;

        public EnumColumn(IDictionary<string, object> properties)
        {
            //Check.IsNotNull(properties, nameof(properties));

            this.properties = properties;
        }

        public IDictionary<string, object> GetProperties() => properties;

        public override void Build(DbCommandBuilder builder, IServiceProvider serviceProvider)
        {
            serviceProvider.GetRequiredService<ColumnFragmentBuilder>()
                .Build((ITableSource)properties["Table"], (string)properties["Name"], builder, serviceProvider);
        }

        public int CompareTo(object other)
        {
            return ((string)properties["Name"]).CompareTo(other);
        }

        public static implicit operator EnumColumn<T>(ColumnBuilder<T> columnBuilder)
        {
            return new EnumColumn<T>(columnBuilder.Properties);
        }
    }
}
