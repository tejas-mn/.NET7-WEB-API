using System;
using System.Text.Json.Serialization;

namespace asp_net_web_api.API.Utility {

    public class Fault : Exception {
        public string? ContextId { get; set; } = Guid.NewGuid().ToString();
        public string? Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Errors { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Warnings { get; set; }
    }
}
