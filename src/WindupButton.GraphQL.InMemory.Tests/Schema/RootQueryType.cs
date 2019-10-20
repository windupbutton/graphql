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
using System.Threading.Tasks;
using WindupButton.GraphQL.Data;
using WindupButton.GraphQL.InMemory.Tests.Data;
using WindupButton.GraphQL.Schema;

namespace WindupButton.GraphQL.InMemory.Tests.Schema
{
    [GraphQLObject("Query")]
    internal class RootQueryType : GraphQLObject
    {
        private readonly IAsyncBatch batch;
        private readonly ObjectDataProvider<OrganisationRecord> organisationProvider;

        public RootQueryType(IAsyncBatch batch)
        {
            var organisations = new List<OrganisationRecord>
            {
                new OrganisationRecord
                {
                    Id = Guid.NewGuid(),
                    Name = "org 1",
                    Users =
                    {
                        new UserRecord
                        {
                            Id = Guid.NewGuid(),
                            Name = "user 1",
                        },
                        new UserRecord
                        {
                            Id = Guid.NewGuid(),
                            Name = "user 2",
                        },
                    },
                },
                new OrganisationRecord
                {
                    Id = Guid.NewGuid(),
                    Name = "org 2",
                    Users =
                    {
                        new UserRecord
                        {
                            Id = Guid.NewGuid(),
                            Name = "user 3",
                        },
                        new UserRecord
                        {
                            Id = Guid.NewGuid(),
                            Name = "user 4",
                        },
                    },
                },
            };

            organisationProvider = batch.Add(new ObjectDataProvider<OrganisationRecord>(token => Task.FromResult(organisations.AsEnumerable())));
            this.batch = batch;
        }

        [Field]
        public IValueAccessor<GraphQLNotNull<OrganisationType>> Organisations() => organisationProvider
            .Select(x => x, new OrganisationType(organisationProvider));

        [Field]
        public IValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<GraphQLString>>>> Values(Input<GraphQLNotNull<GraphQLList<GraphQLNotNull<GraphQLString>>>> values) =>
            batch.Add(new ObjectDataProvider<string>(token => Task.FromResult(values.ListValue())))
                .SelectList<GraphQLString>(x => x);
    }
}
