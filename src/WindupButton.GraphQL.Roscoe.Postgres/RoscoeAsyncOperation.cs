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
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WindupButton.GraphQL.Data;
using WindupButton.Roscoe;
using WindupButton.Roscoe.Expressions;
using WindupButton.Roscoe.Infrastructure;
using WindupButton.Roscoe.Postgres;
using WindupButton.Roscoe.Postgres.Commands;

namespace WindupButton.GraphQL.Roscoe.Postgres
{
    public class RoscoeAsyncOperation<TDb> : IAsyncOperation
        where TDb : RoscoeDb
    {
        private readonly TDb db;
        private readonly List<IWrapper<PostgresQueryCommand>> commands;
        private readonly List<Func<DbCommandResult, object>> queryResults;

        private List<object> result;

        public RoscoeAsyncOperation(TDb db)
        {
            Check.IsNotNull(db, nameof(db));

            this.db = db;
            commands = new List<IWrapper<PostgresQueryCommand>>();
            queryResults = new List<Func<DbCommandResult, object>>();
        }

        public QueryBuilder<TWrapper> Query<TWrapper>(Func<TDb, IWrapper<TWrapper>> commandFactory)
            where TWrapper : PostgresQueryCommand, IWrapper<SelectClause>
        {
            Check.IsNotNull(commandFactory, nameof(commandFactory));

            var command = commandFactory(db);
            commands.Add(command);

            var index = commands.Count - 1;
            var queryResult = new RowGroup(index.ToString(), Enumerable.Empty<string>(), true);
            queryResults.Add(x => queryResult.Apply(x));

            return new QueryBuilder<TWrapper>(command, () => result?[index], queryResult);
        }

        public JsonQueryBuilder QueryJson()
        {
            var json = db.Functions.JsonBuildObject();

            return QueryJson(db.Query().Select(json), new PostgresJsonQueryBuilderAdapter(db, json));
        }

        public JsonQueryBuilder QueryJson<TWrapper>(
            IWrapper<TWrapper> command,
            JsonQueryBuilderAdapter adapter)
            where TWrapper : PostgresQueryCommand, IWrapper<SelectClause>
        {
            Check.IsNotNull(command, nameof(command));

            commands.Add(command);

            var index = commands.Count - 1;
            queryResults.Add(x =>
            {
                // Join together multiple rows to handle SQL Server's insanity of splitting JSON (& XML) into 2033 character lines
                var result = x == null ? null : string.Join("", x.Select(y => y.FirstOrDefault().Value));

                return result == null
                    ? null
                    : JsonConvert.DeserializeObject<JToken>(result.ToString());
            });

            return new JsonQueryBuilder(adapter, () => result?[index]);
        }

        public async Task ExecuteAsync(CancellationToken token)
        {
            result = (await db.ExecuteAsync(token, commands.ToArray()))
                .Zip(queryResults, (x, y) => y(x))
                .ToList();
        }
    }
}
