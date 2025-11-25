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
        [JsonPropertyName("hoursByDay")]
        public List<DailyHours> HoursByDay { get; set; } = new List<DailyHours>();

        // Helper to always have all 7 days
        public static WeeklyOperatingHours CreateDefault()
        {
            return new WeeklyOperatingHours
            {
                HoursByDay = new List<DailyHours>
                {
                    new DailyHours { Day = "monday",    IsOpen = true,  StartTime = "08:00", EndTime = "18:00" },
                    new DailyHours { Day = "tuesday",   IsOpen = true,  StartTime = "08:00", EndTime = "18:00" },
                    new DailyHours { Day = "wednesday", IsOpen = true,  StartTime = "08:00", EndTime = "18:00" },
                    new DailyHours { Day = "thursday",  IsOpen = true,  StartTime = "08:00", EndTime = "18:00" },
                    new DailyHours { Day = "friday",    IsOpen = true,  StartTime = "08:00", EndTime = "18:00" },
                    new DailyHours { Day = "saturday",  IsOpen = true,  StartTime = "09:00", EndTime = "17:00" },
                    new DailyHours { Day = "sunday",    IsOpen = false, StartTime = "09:00", EndTime = "17:00" },
                }
            };
        }
    }
}
