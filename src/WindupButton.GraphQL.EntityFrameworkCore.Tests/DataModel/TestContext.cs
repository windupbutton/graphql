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

using Microsoft.EntityFrameworkCore;

namespace WindupButton.GraphQL.EntityFrameworkCore.Tests.DataModel
{
    public class TestContext : DbContext
    {
        public TestContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var organisation = modelBuilder.Entity<OrganisationRecord>();

            organisation
                .Property(x => x.Name)
                .IsRequired();

            organisation
                .HasMany(x => x.Users)
                .WithOne(x => x.Organisation)
                .HasForeignKey(x => x.OrganisationId)
                .OnDelete(DeleteBehavior.Cascade);

            var user = modelBuilder.Entity<UserRecord>();

            user
                .Property(x => x.Name)
                .IsRequired();

            user
                .HasOne(x => x.Organisation)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.OrganisationId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
