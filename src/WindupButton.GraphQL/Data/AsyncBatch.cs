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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WindupButton.GraphQL.Data
{
    internal sealed class AsyncBatch : IAsyncBatch
    {
        private readonly bool parallelExecution;
        private readonly List<IAsyncOperation> operations;

        public AsyncBatch(bool parallelExecution)
        {
            this.parallelExecution = parallelExecution;

            operations = new List<IAsyncOperation>();
        }

        public TAsyncOperation Add<TAsyncOperation>(TAsyncOperation operation)
            where TAsyncOperation : IAsyncOperation
        {
            operations.Add(operation);

            return operation;
        }

        public async Task ExecuteAsync(CancellationToken token)
        {
            if (parallelExecution)
            {
                await Task.WhenAll(operations
                    .Select(x => x.ExecuteAsync(token))
                    .ToArray());
            }
            else
            {
                foreach (var operation in operations)
                {
                    await operation.ExecuteAsync(token);
                }
            }
        }
    }
}
