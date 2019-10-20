﻿// Copyright 2019 Windup Button
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
using WindupButton.GraphQL.InMemory.Tests.Data;
using WindupButton.GraphQL.Schema;

namespace WindupButton.GraphQL.InMemory.Tests.Schema
{
    [GraphQLObject("Organisation")]
    internal sealed class OrganisationType : GraphQLObject
    {
        private readonly ObjectDataProvider<OrganisationRecord> provider;

        public OrganisationType(ObjectDataProvider<OrganisationRecord> provider)
        {
            this.provider = provider;
        }

        [Field]
        public IValueAccessor<GraphQLNotNull<GraphQLID>> Id() => provider
            .Select<GraphQLID>(x => x.Id);

        [Field]
        public IValueAccessor<GraphQLNotNull<GraphQLString>> Name() => provider
            .Select<GraphQLString>(x => x.Name);

        [Field]
        public IValueAccessor<GraphQLNotNull<GraphQLList<GraphQLNotNull<UserType>>>> Users() => provider
            .SelectList(x => x.Users, new UserType(provider.CreateChildProviderForCollection(x => x.Users)));
    }
}
