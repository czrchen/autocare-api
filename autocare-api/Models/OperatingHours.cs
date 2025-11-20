using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace autocare_api.Models
{
    public class DailyHours
    {
        [JsonPropertyName("day")]
        public string Day { get; set; } = "";

        [JsonPropertyName("isOpen")]
        public bool IsOpen { get; set; }

        [JsonPropertyName("startTime")]
        public string? StartTime { get; set; }

        [JsonPropertyName("endTime")]
        public string? EndTime { get; set; }
    }

    public class WeeklyOperatingHours
    {
        public List<DailyHours> HoursByDay { get; set; } = new List<DailyHours>();
    }
}
