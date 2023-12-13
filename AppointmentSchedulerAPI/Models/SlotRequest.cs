namespace AppointmentSchedulerAPI.Models;

public class SlotRequest
{
    public string? FacilityId { get; set; }
    public string? Start { get; set; }
    public string? End { get; set; }
    public string? Comments { get; set; }
    public Patient? Patient { get; set; }
}