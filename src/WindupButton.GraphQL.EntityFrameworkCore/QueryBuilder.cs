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
using System.Linq.Expressions;
using WindupButton.GraphQL.Data;
using WindupButton.GraphQL.Schema;

namespace WindupButton.GraphQL.EntityFrameworkCore
{
    internal sealed class QueryBuilder<T> : IQueryBuilder<T>
    {
        private readonly IQueryable<T> queryable;

        public QueryBuilder(IQueryable<T> queryable)
        {
            Check.IsNotNull(queryable, nameof(queryable));

            this.queryable = queryable;
        }

        public IQueryBuilder<TOther> Query<TOther>(Expression<Func<T, TOther>> selectionExpression)
        {
            throw new NotImplementedException();
        }

        public IQueryBuilder<TOther> QueryMany<TOther>(Expression<Func<T, IEnumerable<TOther>>> selectionExpression)
        {
            throw new NotImplementedException();
        }

        public IValueAccessor Select<TProp>(Expression<Func<T, TProp>> selectionExpression)
        {
            throw new NotImplementedException();
        }

        public IValueAccessor<TOther> Select<TOther>(TOther other)
            where TOther : GraphQLObject
        {
            throw new NotImplementedException();
        }
    }
}
