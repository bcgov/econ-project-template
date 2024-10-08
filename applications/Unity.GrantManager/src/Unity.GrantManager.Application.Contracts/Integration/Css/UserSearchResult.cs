﻿using System.Text.Json.Serialization;

namespace Unity.GrantManager.Integration.Css
{
    public class UserSearchResult
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("err")]
        public string? Error { get; set; }

        [JsonPropertyName("data")]
        public CssUser[]? Data { get; set; }
    }
}
