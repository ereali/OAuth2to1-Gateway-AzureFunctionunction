//Edward Reali | Sooner Card - University of Oklahoma
//Copyright Edward Reali 2022

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DoorOverrideAPI.Models
{
    public class SessionTokenResponse
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("ResultDomain")]
        public int ResultDomain { get; set; }

        [JsonProperty("ResultDomainId")]
        public int ResultDomainId { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("SessionToken")]
        public string SessionToken { get; set; }
    }
}
