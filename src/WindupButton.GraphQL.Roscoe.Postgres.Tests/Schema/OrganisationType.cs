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
using WindupButton.GraphQL.Roscoe.Postgres.Tests.Data;
using WindupButton.GraphQL.Schema;
using WindupButton.Roscoe;
using WindupButton.Roscoe.Expressions;
using WindupButton.Roscoe.Postgres;

namespace WindupButton.GraphQL.Roscoe.Postgres.Tests.Schema
{
    [GraphQLObject("Organisation")]
    internal sealed class OrganisationType : GraphQLObject
    {
        private readonly IAsyncBatch batch;
        private readonly RoscoeDb db;
        private readonly OrganisationTable organisations;
        private readonly JsonQueryBuilder queryBuilder;

        public OrganisationType(
            IAsyncBatch batch,
            RoscoeDb db,
            OrganisationTable organisations,
            JsonQueryBuilder queryBuilder)
        {
            this.batch = batch;
            this.db = db;
            this.organisations = organisations;
            this.queryBuilder = queryBuilder;
        }

        [Field]
        public IValueAccessor<GraphQLNotNull<GraphQLID>> Id() =>
            queryBuilder.Id(organisations.Id);

        [Field]
        public IValueAccessor<GraphQLNotNull<GraphQLString>> Name() =>
            queryBuilder.String(organisations.Name);

        [Field]
        public IValueAccessor<GraphQLNotNull<GraphQLFloat>> RevenuePerAnum() =>
            queryBuilder.Float(organisations.RevenuePerAnum);

        [Field]
        public IValueAccessor<GraphQLNotNull<GraphQLEnum<OrgType>>> Type() =>
            queryBuilder.Enum(organisations.OrgType);

        [Field]
        public IValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<UserType>>>> Users()
        {
            var users = new UserTable("u");

            return queryBuilder.SelectList(
                db.Query()
                    .From(users)
                    .Where(users.OrganisationId == organisations.Id),
                x => new UserType(batch, db, users, x));
        }

        [Field]
        public IValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<GraphQLString>>>> UserNames()
        {
            var users = new UserTable("ids_u");

            return queryBuilder.SelectScalarList<GraphQLString, string>(
                db.Query()
                    .From(users)
                    .Where(users.OrganisationId == organisations.Id),
                users.Name);
        }

        [Field]
        public IValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<GraphQLString>>>> Departments() =>
            queryBuilder.SelectScalarList<GraphQLString, string>(organisations.Departments);

        [Field]
        public IValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<GraphQLDate>>>> ClosedPeriods() =>
            queryBuilder.SelectScalarList<GraphQLDate, DateTime?>(new DbArrayWrapper<DateTime?>(organisations.ClosedPeriods));
    }
}
