using System;
using System.Net;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace asp_net_web_api.API.Utility {

    public class Fault  {
        public Fault(string? message, HttpStatusCode code, string? stackTrace = null) 
        {
            ErrorMessage = message;
            ErrorTrace = stackTrace;
            StatusCode = code;
        }

        public HttpStatusCode? StatusCode { get; set; }
        public string? ErrorMessage { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string? ErrorTrace{ get; set; }

        // [Newtonsoft.Json.JsonIgnore]
        public List<string>? Errors { get; set; }
        
       [Newtonsoft.Json.JsonIgnore]
        public List<string>? Warnings { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
