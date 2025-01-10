using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace URLShortener_client
{
    public class ResponseObject
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
         [JsonPropertyName("shortUrl")]
        public string ShortUrl { get; set; }
    }

    public class ResponseModel
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("responseObject")]
        public ResponseObject Response { get; set; }

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; }
    }
}
