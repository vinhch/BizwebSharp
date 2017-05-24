﻿using System;
using Newtonsoft.Json;

namespace BizwebSharp.Entities
{
    public class BaseEntityCanPublishable : BaseEntityWithTimeline
    {
        [JsonProperty("published_on", DefaultValueHandling = DefaultValueHandling.Include,
             NullValueHandling = NullValueHandling.Include)]
        public DateTime? PublishedOn { get; set; }
    }
}