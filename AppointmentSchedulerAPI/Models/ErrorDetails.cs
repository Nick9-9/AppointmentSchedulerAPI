namespace AppointmentSchedulerAPI.Models;

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public required string Message { get; set; }
}