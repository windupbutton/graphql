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
using Newtonsoft.Json;

namespace WindupButton.GraphQL.Data
{
    public sealed class GraphQLError
    {
        public GraphQLError(
            string message,
            IEnumerable<GraphQLLocation> locations = null,
            IEnumerable<object> path = null,
            IDictionary<string, object> extensions = null)
        {
            Check.IsNotNullOrWhiteSpace(message, nameof(message));

            Message = message;
            Locations = locations?.Any() ?? false ? locations : null;
            Path = path?.Any() ?? false ? path : null;
            Extensions = extensions;
        }

        [JsonProperty("message")]
        public string Message { get; }

        [JsonProperty("locations", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<GraphQLLocation> Locations { get; }

        [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<object> Path { get; }

        [JsonProperty("extensions", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, object> Extensions { get; }
    }
}
