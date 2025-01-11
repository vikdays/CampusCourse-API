﻿using System.Text.Json.Serialization;

namespace api.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StudentStatuses
    {
        InQueue, 
        Accepted,
        Declined
    };
}

