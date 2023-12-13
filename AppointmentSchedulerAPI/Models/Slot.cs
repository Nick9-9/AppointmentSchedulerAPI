namespace AppointmentSchedulerAPI.Models;

public class Slot
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public static Slot FromTimeSpan(TimeSpan slotStart, int interval, DateTime date)
    {
        var start = date.Add(slotStart);

        return new Slot
        {
            Start = start,
            End = start.AddMinutes(interval)
        };
    }
}