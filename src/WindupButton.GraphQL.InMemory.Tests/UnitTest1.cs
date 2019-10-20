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
using System.Threading.Tasks;
using Newtonsoft.Json;
using WindupButton.GraphQL.InMemory.Tests.Data;
using WindupButton.GraphQL.InMemory.Tests.Schema;
using WindupButton.GraphQL.Schema;
using Xunit;

namespace WindupButton.GraphQL.InMemory.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            // arrange
            var organisation = new OrganisationRecord
            {
                Id = Guid.NewGuid(),
                Name = "Organisation 1",
                Users = new List<UserRecord>
                {
                    new UserRecord
                    {
                        Id = Guid.NewGuid(),
                        Name = "User 1",
                    },
                    new UserRecord
                    {
                        Id = Guid.NewGuid(),
                        Name = "User 2",
                    },
                },
            };
            var schema = GraphQLSchema.Create(x => new RootQueryType(x));

            // act
            var result = await schema.ExecuteRequestAsync(@"query {
                Organisations {
                    id: Id
                    Name
                    Users {
                        Id
                        Name
                    }
                }
            }", null, null);

            // assert
            Assert.Equal(
                JsonConvert.SerializeObject(new
                {
                    id = organisation.Id,
                    organisation.Name,
                    organisation.Users,
                }),
                JsonConvert.SerializeObject(result)
            );
        }

        [Fact]
        public async Task Test2()
        {
            // arrange
            var organisation = new OrganisationRecord
            {
                Id = Guid.NewGuid(),
                Name = "Organisation 1",
                Users = new List<UserRecord>
                {
                    new UserRecord
                    {
                        Id = Guid.NewGuid(),
                        Name = "User 1",
                    },
                    new UserRecord
                    {
                        Id = Guid.NewGuid(),
                        Name = "User 2",
                    },
                },
            };
            var schema = GraphQLSchema.Create(x => new RootQueryType(x));

            // act
            var result = await schema.ExecuteRequestAsync(@"query($values: [String!]!) {
                Values(values: $values)
            }",
            new Dictionary<string, object>
            {
                { "values", new[] { "one", "two" } },
            },
            null);

            // assert
            Assert.Equal(
                JsonConvert.SerializeObject(new
                {
                    id = organisation.Id,
                    organisation.Name,
                    organisation.Users,
                }),
                JsonConvert.SerializeObject(result)
            );
        }
    }
}
