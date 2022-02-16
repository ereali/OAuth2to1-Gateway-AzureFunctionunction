//Edward Reali | Sooner Card - University of Oklahoma
//Copyright Edward Reali 2022

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DoorOverrideAPI.Models
{
    public class SessionTokenRequest
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("Password")]
        public string Password { get; set; }

        [JsonProperty("WorkstationId")]
        public int WorkstationId { get; set; }

        [JsonProperty("IsSuperUser")]
        public bool IsSuperUser { get; set; }
    }
}

