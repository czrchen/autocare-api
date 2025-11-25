namespace autocare_api.DTOs
{
    public class DailyHoursDto
    {
        public string Day { get; set; } = "";
        public bool IsOpen { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
    }

    public class WeeklyOperatingHoursDto
    {
        public List<DailyHoursDto> HoursByDay { get; set; } = new();
    }
}
