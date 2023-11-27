using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace asp_net_web_api.API.Utility {

    public class Fault  {
        public Fault(string? message, string? stackTrace = null) 
        {
            ErrorMessage = message;
            ErrorTrace = stackTrace;
        }

        public string? ContextId { get; set; } = Guid.NewGuid().ToString();
        public string? ErrorMessage { get; set; }

        public string? ErrorTrace{ get; set; }

        public  List<string>? Errors { get; set; }
        
        public  List<string>? Warnings { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
