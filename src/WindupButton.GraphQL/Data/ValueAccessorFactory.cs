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
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace WindupButton.GraphQL.Data
{
    internal sealed class ValueAccessorFactory
    {
        private readonly object target;
        private readonly MethodInfo methodInfo;

        public ValueAccessorFactory(object target, MethodInfo methodInfo)
        {
            Check.IsNotNull(target, nameof(target));
            Check.IsNotNull(methodInfo, nameof(methodInfo));

            this.target = target;
            this.methodInfo = methodInfo;
        }

        // these values have been coerced & there is a value for each parameter
        public async Task<IValueAccessor> Create(IDictionary<string, object> values, CancellationToken token)
        {
            var parameters = methodInfo.GetParameters();
            var inputs = new List<object>();

            foreach (var parameter in parameters)
            {
                if (typeof(IInput).IsAssignableFrom(parameter.ParameterType))
                {
                    inputs.Add(Input.Create(
                        parameter.ParameterType,
                        values[parameter.Name]));
                }
                else if (typeof(CancellationToken).IsAssignableFrom(parameter.ParameterType))
                {
                    inputs.Add(token);
                }
                else
                {
                    throw new InvalidOperationException(); // todo message
                }
            }

            var result = methodInfo.Invoke(target, inputs.ToArray());

            if (methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                await (Task)result;

                return (IValueAccessor)methodInfo.ReturnType.GetProperty(nameof(Task<object>.Result)).GetValue(result);
            }

            return (IValueAccessor)result;
        }
    }
}
