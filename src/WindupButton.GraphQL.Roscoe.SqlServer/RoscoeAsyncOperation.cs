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
using WindupButton.Roscoe.SqlServer.Commands;

namespace WindupButton.GraphQL.Roscoe.SqlServer
{
    public class RoscoeAsyncOperation<TDb> : IAsyncOperation
        where TDb : RoscoeDb
    {
        private readonly TDb db;
        private readonly List<IWrapper<SqlServerQueryCommand>> commands;
        private readonly List<Func<DbCommandResult, object>> queryResults;

        private List<object> result;

        public RoscoeAsyncOperation(TDb db)
        {
            Check.IsNotNull(db, nameof(db));

            this.db = db;
            commands = new List<IWrapper<SqlServerQueryCommand>>();
            queryResults = new List<Func<DbCommandResult, object>>();
        }

        public QueryBuilder<TWrapper> Query<TWrapper>(Func<TDb, IWrapper<TWrapper>> commandFactory)
            where TWrapper : SqlServerQueryCommand, IWrapper<SelectClause>
        {
            Check.IsNotNull(commandFactory, nameof(commandFactory));

            var command = commandFactory(db);
            commands.Add(command);

            var index = commands.Count - 1;
            var queryResult = new RowGroup(index.ToString(), Enumerable.Empty<string>(), true);
            queryResults.Add(x => queryResult.Apply(x));

            return new QueryBuilder<TWrapper>(command, () => result?[index], queryResult);
        }

        public JsonQueryBuilder QueryJson<TWrapper>(
            Func<TDb, IWrapper<TWrapper>> commandFactory,
            Func<IWrapper<TWrapper>, JsonQueryBuilderAdapter> adapter)
            where TWrapper : SqlServerQueryCommand, IWrapper<SelectClause>
        {
            Check.IsNotNull(commandFactory, nameof(commandFactory));

            var command = commandFactory(db);
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

            return new JsonQueryBuilder(adapter(command), () => result?[index]);
        }

        public async Task ExecuteAsync(CancellationToken token)
        {
            result = (await db.ExecuteAsync(token, commands.ToArray()))
                .Zip(queryResults, (x, y) => y(x))
                .ToList();
        }
    }
}
