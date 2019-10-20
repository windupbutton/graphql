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

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using WindupButton.GraphQL.Roscoe.SqlServer.Tests.Data;
using WindupButton.GraphQL.Roscoe.SqlServer.Tests.Schema;
using WindupButton.GraphQL.Schema;
using WindupButton.Roscoe;
using WindupButton.Roscoe.SqlServer;
using Xunit;

namespace WindupButton.GraphQL.Roscoe.SqlServer.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task TestJson()
        {
            var organisations = new OrganisationTable("orgs");
            var users = new UserTable();

            var services = new ServiceCollection()
                .AddRoscoeSqlServer("Server=localhost;Database=test;");

            using (var provider = services.BuildServiceProvider())
            {
                var db = new RoscoeDb(provider);
                var schema = GraphQLSchema.Create(x => new RootQueryType(x, db));
                var result = await schema.ExecuteRequestAsync(@"query {
    Organisations {
        id: Id
        Name
        UserNames
        Users {
            Id
            Name
            Organisation {
                Name
            }
        }
    }
}", null);

                var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            }
        }

        [Fact]
        public async Task TestStrongTypeQuery()
        {
            var services = new ServiceCollection()
                .AddRoscoeSqlServer("Server=localhost;Database=test;");

            using (var provider = services.BuildServiceProvider())
            {
                var db = new RoscoeDb(provider);
                var schema = GraphQLSchema.Create(x => new RootQueryType(x, db));

                var result = await schema.QueryAsync(x => new
                {
                    Organisations = x.Organisations()
                        .Select(y => new
                        {
                            //Foo = y.Id().GetValue(null),
                            Id = y.Id().Value() + "foo",
                            Name = y.Name().Value(),
                            Type = y.Type().Value(),
                            Users = y.Users()
                                .Select(z => new
                                {
                                    Org = z.Organisation()
                                        .Select(a => new
                                        {
                                            Name = a.Name().Value(),
                                        }),
                                }),
                        }),
                });

                /*
                schema.ExecuteQueryAsync((x, dr) => new
                {
                    Organisations = GraphQLSchemaOperations.Select(x.Organisations(), dr, (y, dr2) => new
                    {
                        Id = (string)y.Id.GetValue(dr2),
                        Name = (string)y.Name.GetValue(dr2),
                        Users = GraphQLSchemaOperations.Select(y.Users(), dr2, (z, dr3) => new
                        {
                            Org = GraphQLSchemaOperations.Select(z.Organisation(), dr3, (a, dr4) => new
                            {
                                Name = (string)a.Name().GetValue(dr4),
                            },
                        },
                    }),
                });

                 */

                var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            }
        }

        /*
        [Fact]
        public async Task TestJson2()
        {
            var organisations = new OrganisationTable("orgs");
            var users = new UserTable();

            var options = new DbOptions()
                .UsePostgres("Server=localhost;Database=test;");

            using (var db = new RoscoeDb(options))
            {
                var schema = new GraphQLSchema(x => new RootQueryType(x, db), null);
                var result = await schema.ExecuteRequestAsync(@"query {
    OrganisationNames
}", null);

                var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            }
        }

        [Fact]
        public async Task TestNull()
        {
            var organisations = new OrganisationTable("orgs");
            var users = new UserTable();

            var options = new DbOptions()
                .UsePostgres("Server=localhost;Database=test;");

            using (var db = new RoscoeDb(options))
            {
                var schema = new GraphQLSchema(x => new RootQueryType(x, db), null);
                var result = await schema.ExecuteRequestAsync(@"query($organisationId: ID!) {
    Organisation(organisationId: $organisationId) {
        Name
    }
}", new Dictionary<string, object> { { "organisationId", Guid.NewGuid() } });

                var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            }
        }

        [Fact]
        public async Task TestEnum()
        {
            var organisations = new OrganisationTable("orgs");
            var users = new UserTable();

            var options = new DbOptions()
                .UsePostgres("Server=localhost;Database=test;");

            using (var db = new RoscoeDb(options))
            {
                var schema = new GraphQLSchema(x => new RootQueryType(x, db), null);
                var result = await schema.ExecuteRequestAsync(@"query($value: OrgType) {
    EnumValue(value: $value)
}", new Dictionary<string, object> { { "value", OrgType.Corporate.ToString().ToLower() } });

                var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            }
        }
        */
        //[Fact]
        //public async Task TestStrongQuery()
        //{
        //    var organisations = new OrganisationTable("orgs");
        //    var users = new UserTable();

        //    var options = new DbOptions()
        //        .UsePostgres("Server=localhost;Database=test;");

        //    using (var db = new RoscoeDb(options))
        //    {
        //        var schema = new GraphQLSchema(x => new RootQueryType(x, db), null);
        //        var result = await GraphQLSchema.Query(x => new RootQueryType(x, db))
        //            .SelectAsync((query, v) => new
        //            {
        //                OrganisationNames = v.ListValue(query.OrganisationNames()),
        //            });

        //        var json = JsonConvert.SerializeObject(result, Formatting.Indented);
        //    }
        //}
    }
}
