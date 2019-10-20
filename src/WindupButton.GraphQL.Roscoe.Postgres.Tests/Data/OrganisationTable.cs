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
using WindupButton.GraphQL.Roscoe.Postgres.Tests.Schema;
using WindupButton.Roscoe.Expressions;
using WindupButton.Roscoe.Postgres.Schema;
using WindupButton.Roscoe.Schema;

namespace WindupButton.GraphQL.Roscoe.Postgres.Tests.Data
{
    internal sealed class OrganisationTable : PostgresTable
    {
        public OrganisationTable(string alias = default)
            : base("organisations", "public", alias)
        {
        }

        public GuidColumn Id => Column()
            .OfType(new UUIDColumnType());

        //.NotNullable;

        public StringColumn Name => Column()
            .OfType(new VarCharColumnType(300));

        public EnumColumn<OrgType> OrgType => Column()
            .OfType(new EnumColumnType<OrgType>());

        public DecimalColumn RevenuePerAnum => Column()
            .OfType(new DecimalColumnType(18, 6));

        //.NotNullable;

        public DbArray<string> Departments => Column()
            .OfType(new ArrayColumnType<VarCharColumnType, string>(new VarCharColumnType(300)));

        public DbArray<DateTime> ClosedPeriods => Column()
            .OfType(new ArrayColumnType<TimestampColumnType, DateTime>(new TimestampColumnType(false)));
    }
}
