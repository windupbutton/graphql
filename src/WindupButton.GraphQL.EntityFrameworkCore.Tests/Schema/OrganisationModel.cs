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

using WindupButton.GraphQL.EntityFrameworkCore.Tests.DataModel;
using WindupButton.GraphQL.Schema;

namespace WindupButton.GraphQL.EntityFrameworkCore.Tests.Schema
{
    [GraphQLObject("Organisation")]
    public class OrganisationModel : GraphQLObject
    {
        private readonly IQueryBuilder<OrganisationRecord> queryBuilder;

        public OrganisationModel(IQueryBuilder<OrganisationRecord> queryBuilder)
        {
            this.queryBuilder = queryBuilder;
        }

        //[Field]
        //public Field Id() =>
        //    .NotNull
        //    .Scalar<GraphQLID>(() => queryBuilder.Select(x => x.Id));

        //[Field]
        //public Field Name() => Field()
        //    .NotNull
        //    .Scalar<GraphQLString>(() => queryBuilder.Select(x => x.Name));

        //[Field]
        //public Field Users() => Field()
        //    .NotNull
        //    .ListOf
        //    .NotNull
        //    .Object(() => queryBuilder.Select(new UserModel(queryBuilder.QueryMany(x => x.Users))));
    }
}
