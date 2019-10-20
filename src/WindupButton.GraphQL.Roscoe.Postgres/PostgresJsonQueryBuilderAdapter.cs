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

using WindupButton.Roscoe;
using WindupButton.Roscoe.Expressions;
using WindupButton.Roscoe.Postgres;
using WindupButton.Roscoe.Postgres.Commands;
using WindupButton.Roscoe.Postgres.Expressions;
using WindupButton.Roscoe.Postgres.Schema;

namespace WindupButton.GraphQL.Roscoe.Postgres
{
    public sealed class PostgresJsonQueryBuilderAdapter : JsonQueryBuilderAdapter
    {
        private readonly RoscoeDb db;
        private readonly JsonBuildObjectFragment buildObject;

        public PostgresJsonQueryBuilderAdapter(RoscoeDb db, JsonBuildObjectFragment buildObject)
        {
            Check.IsNotNull(db, nameof(db));
            Check.IsNotNull(buildObject, nameof(buildObject));

            this.db = db;
            this.buildObject = buildObject;
        }

        public override void Select(string name, IDbFragment value)
        {
            buildObject.AddMember(name, value);
        }

        public override void Select(string name, IWrapper<PostgresQueryCommand> childQuery, IDbFragment value)
        {
            childQuery.Select(db.Functions.Coalesce(db.Functions.JsonAgg(value), db.Functions.Cast("[]".DbValue(), new JsonColumnType())));

            buildObject.AddMember(name, childQuery.Value);
        }

        public override JsonQueryBuilderAdapter Select(IWrapper<PostgresQueryCommand> childQuery, string name, bool isSingular)
        {
            var childJsonExpression = db.Functions.JsonBuildObject();
            childQuery.Select(
                isSingular
                ? childJsonExpression
                : db.Functions.Coalesce(
                    db.Functions.JsonAgg(childJsonExpression),
                    db.Functions.Cast("[]".DbValue(), new JsonColumnType())));

            buildObject.AddMember(name, childQuery.Value);

            return new PostgresJsonQueryBuilderAdapter(db, childJsonExpression);
        }
    }
}
