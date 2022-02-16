using System;
using Newtonsoft.Json;
using System.Collections.Generic;

public class APIResponse
{
    [JsonProperty("RequestId")]
    public string RequestId { get; set; }

    [JsonProperty("DoorIds")]
    public List<int> DoorIds { get; set; }

    [JsonProperty("StateOverrideValue")]
    public int StateOverrideValue { get; set; }

    [JsonProperty("DurationOverrideValue")]
    public int DurationOverrideValue { get; set; }

    [JsonProperty("Success")]
    public bool Success { get; set; }
}