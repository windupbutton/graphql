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

using WindupButton.GraphQL.Data;
using WindupButton.GraphQL.Roscoe.Postgres.Tests.Data;
using WindupButton.GraphQL.Schema;
using WindupButton.Roscoe;
using WindupButton.Roscoe.Postgres;

namespace WindupButton.GraphQL.Roscoe.Postgres.Tests.Schema
{
    [GraphQLObject("User")]
    internal sealed class UserType : GraphQLObject
    {
        private readonly IAsyncBatch batch;
        private readonly RoscoeDb db;
        private readonly UserTable users;
        private readonly JsonQueryBuilder queryBuilder;

        public UserType(IAsyncBatch batch, RoscoeDb db, UserTable users, JsonQueryBuilder queryBuilder)
        {
            this.batch = batch;
            this.db = db;
            this.users = users;
            this.queryBuilder = queryBuilder;
        }

        [Field]
        public IValueAccessor<GraphQLNotNull<GraphQLID>> Id() =>
            queryBuilder.Id(users.Id);

        [Field]
        public IValueAccessor<GraphQLNotNull<GraphQLString>> Name() =>
            queryBuilder.String(users.Name);

        [Field]
        public IValueAccessor<GraphQLNotNull<OrganisationType>> Organisation()
        {
            var organisations = new OrganisationTable("uo");

            return queryBuilder.Select(
                db.Query()
                    .From(organisations)
                    .Where(users.OrganisationId == organisations.Id),
                x => new OrganisationType(batch, db, organisations, x));
        }
    }
}
