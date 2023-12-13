namespace AppointmentSchedulerAPI.Models;

public class BusyPeriod
{
    public WorkPeriod? WorkPeriod { get; set; }
    public List<Slot>? BusySlots { get; set; }
}