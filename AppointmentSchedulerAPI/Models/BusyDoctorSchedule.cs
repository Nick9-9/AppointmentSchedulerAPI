using System.Text.Json.Serialization;

namespace AppointmentSchedulerAPI.Models;

public class BusyDoctorSchedule
{
    public Facility? Facility { get; set; }
    public int SlotDurationMinutes { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BusyPeriod? Monday { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BusyPeriod? Tuesday { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BusyPeriod? Wednesday { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BusyPeriod? Thursday { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BusyPeriod? Friday { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BusyPeriod? Saturday { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BusyPeriod? Sunday { get; set; }

    [JsonIgnore]
    public Dictionary<DayOfWeek, BusyPeriod?> Days =>
        new()
        {
            { DayOfWeek.Monday, Monday },
            { DayOfWeek.Tuesday, Tuesday },
            { DayOfWeek.Wednesday, Wednesday },
            { DayOfWeek.Thursday, Thursday },
            { DayOfWeek.Friday, Friday },
            { DayOfWeek.Saturday, Saturday },
            { DayOfWeek.Sunday, Sunday }
        };
}