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
using System.Linq;
using System.Threading.Tasks;
using WindupButton.GraphQL.Data;
using WindupButton.GraphQL.InMemory;
using WindupButton.GraphQL.Roscoe.SqlServer.Tests.Data;
using WindupButton.GraphQL.Schema;
using WindupButton.Roscoe;
using WindupButton.Roscoe.SqlServer;

namespace WindupButton.GraphQL.Roscoe.SqlServer.Tests.Schema
{
    [GraphQLObject("Query")]
    internal sealed class RootQueryType : GraphQLObject
    {
        private readonly IAsyncBatch batch;
        private readonly RoscoeDb db;

        private readonly RoscoeAsyncOperation<RoscoeDb> provider;
        private readonly ObjectDataProvider<OrgType> enumProvider;

        public RootQueryType(IAsyncBatch batch, RoscoeDb db)
        {
            this.batch = batch;
            this.db = db;

            provider = batch.Add(new RoscoeAsyncOperation<RoscoeDb>(db));
            enumProvider = batch.Add(new ObjectDataProvider<OrgType>(token => Task.FromResult(OrgType.Sme)));
        }

        [Field]
        public IValueAccessor<GraphQLNotNull<OrganisationType>> Organisation(Input<GraphQLNotNull<GraphQLID>> organisationId)
        {
            var organisations = new OrganisationTable("o");

            return provider
                .QueryJson(
                    x => x.Query().ForJsonPathWithoutArrayWrapper(),
                    x => new SqlServerJsonQueryBuilderAdapter(db, x))
                .Select(
                    db.Query()
                        .From(organisations)
                        .Where(organisations.Id == Guid.Parse(organisationId.Value())),
                    x => new OrganisationType(batch, db, organisations, x));
        }

        [Field]
        public IValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<OrganisationType>>>> Organisations()
        {
            var organisations = new OrganisationTable("o");

            return provider
                .QueryJson(
                    x => x.Query().ForJsonPathWithoutArrayWrapper(),
                    x => new SqlServerJsonQueryBuilderAdapter(db, x))
                .SelectList(
                    db.Query()
                        .From(organisations),
                    x => new OrganisationType(batch, db, organisations, x));
        }

        [Field]
        public IValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<GraphQLString>>>> OrganisationNames()
        {
            var organisations = new OrganisationTable("o");

            return provider
                .QueryJson(
                    x => x.Query().ForJsonPathWithoutArrayWrapper(),
                    x => new SqlServerJsonQueryBuilderAdapter(db, x))
                .SelectScalarList<GraphQLString, string>(
                    db.Query()
                        .From(organisations),
                    organisations.Name);
        }

        [Field]
        public IValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<OrganisationType>>>> OrganisationsNamed(Input<GraphQLString> name)
        {
            var organisations = new OrganisationTable("o");

            return provider
                .QueryJson(
                    x => x.Query().ForJsonPathWithoutArrayWrapper(),
                    x => new SqlServerJsonQueryBuilderAdapter(db, x))
                .SelectList(
                    db.Query()
                        .From(organisations)
                        .Where(organisations.Name == name.Value()),
                    x => new OrganisationType(batch, db, organisations, x));
        }

        [Field]
        public IValueAccessor<GraphQLEnum<OrgType>> EnumValue(Input<GraphQLEnum<OrgType>> value)
        {
            return enumProvider.SelectNullable<GraphQLEnum<OrgType>>(x => value.Value());
        }
    }
}
