namespace AppointmentSchedulerAPI.Models;

public class AvailableDoctorSchedule
{
    public Facility? Facility { get; set; }
    public int SlotDurationMinutes { get; set; }
    
    public Dictionary<DayOfWeek, AvailablePeriod?> Days { get; set; }
}