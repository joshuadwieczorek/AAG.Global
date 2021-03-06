using System;
using System.Collections.Generic;

namespace AAG.Global.Health
{
    public struct HealthCheckResponse
    {
        public string Status { get; set; }
        public IEnumerable<HealthCheck> Checks { get; set; }
        public TimeSpan Duration { get; set; }
    }
}