namespace AppointmentSchedulerAPI.Models;

public class AvailablePeriod
{
    public WorkPeriod? WorkPeriod { get; set; }
    public List<Slot>? AvailableSlots { get; set; }
}