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
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WindupButton.GraphQL.EntityFrameworkCore.Tests.DataModel;
using Xunit;

namespace WindupButton.GraphQL.EntityFrameworkCore.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<TestContext>()
                    .UseSqlite(connection);

                using (var context = new TestContext(options.Options))
                {
                    await context.Database.EnsureCreatedAsync();
                }

                var org1Id = Guid.NewGuid();
                var user1Id = Guid.NewGuid();

                using (var context = new TestContext(options.Options))
                {
                    context.Add(new OrganisationRecord
                    {
                        Id = org1Id,
                        Name = "Org 1",
                    });

                    context.Add(new UserRecord
                    {
                        OrganisationId = org1Id,
                        Id = user1Id,
                        Name = "user 1",
                    });

                    await context.SaveChangesAsync();
                }

                using (var context = new TestContext(options.Options))
                {
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
