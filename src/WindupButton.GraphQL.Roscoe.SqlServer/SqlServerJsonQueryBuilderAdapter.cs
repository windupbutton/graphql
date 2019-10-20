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
using WindupButton.Roscoe.SqlServer;
using WindupButton.Roscoe.SqlServer.Commands;

namespace WindupButton.GraphQL.Roscoe.SqlServer
{
    public sealed class SqlServerJsonQueryBuilderAdapter : JsonQueryBuilderAdapter
    {
        private readonly RoscoeDb db;
        private readonly IWrapper<IWrapper<SelectClause>> command;

        public SqlServerJsonQueryBuilderAdapter(RoscoeDb db, IWrapper<IWrapper<SelectClause>> command)
        {
            Check.IsNotNull(db, nameof(db));
            Check.IsNotNull(command, nameof(command));

            this.db = db;
            this.command = command;
        }

        public override void Select(string name, IDbFragment value)
        {
            command.Select(value).As(name);
        }

        public override void Select(string name, string itemName, IWrapper<SqlServerQueryCommand> childQuery, IDbFragment value)
        {
            childQuery
                .ForJsonPath()
                .Select(value)
                .As(itemName);

            //childQuery.Select(DbFunctions.Coalesce(SqlServerDbFunctions.JsonAgg(expression), SqlServerDbFunctions.Cast("[]", new JsonColumnType())));

            command.Select(db.Functions.JsonQuery(db.Functions.Coalesce(childQuery.Value, "[]".DbValue()))).As(name);
        }

        public override JsonQueryBuilderAdapter Select(IWrapper<SqlServerQueryCommand> childQuery, string name, bool isSingular)
        {
            if (isSingular)
            {
                childQuery.ForJsonPathWithoutArrayWrapper();
            }
            else
            {
                childQuery.ForJsonPath();
            }

            if (isSingular)
            {
                command.Select(db.Functions.JsonQuery(childQuery.Value)).As(name);
            }
            else
            {
                command.Select(db.Functions.JsonQuery(db.Functions.Coalesce(childQuery.Value, (DbString)"[]"))).As(name);
            }

            return new SqlServerJsonQueryBuilderAdapter(db, childQuery);
        }
    }
}
