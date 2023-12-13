using AppointmentSchedulerAPI.Contracts;
using AppointmentSchedulerAPI.Models;
using AppointmentSchedulerAPI.Services;
using AppointmentSchedulerAPI.Validation;
using Carter;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AppointmentSchedulerAPI.EndPoints;

public class SlotEndPoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/slots");

        group.MapGet("/available/{dateTime}", GetAvailability).WithName(nameof(GetAvailability));
        group.MapGet("/busy/{dateTime}", GetBusySchedule).WithName(nameof(GetBusySchedule));
        group.MapPost("/take-slot", BookSlot).WithName(nameof(BookSlot));
    }

    public static async Task<Results<Ok<BusyDoctorSchedule>, NotFound<string>>> GetBusySchedule(string dateTime,
        SlotClientService slotClientAvailabilityService)
    {
        var busySchedule = await slotClientAvailabilityService.GetBusySchedule(dateTime);
        return busySchedule.IsSuccess
            ? TypedResults.Ok(busySchedule.Value)
            : TypedResults.NotFound(busySchedule.Error);
    }

    public static async Task<Results<Ok<AvailableDoctorSchedule>, NotFound<string>>> GetAvailability(string dateTime,
        ISlotAvailabilityService slotAvailabilityService)
    {
        var availableSchedule = await slotAvailabilityService.GetAvailableSchedule(dateTime);

        return availableSchedule.IsSuccess
            ? TypedResults.Ok(availableSchedule.Value)
            : TypedResults.NotFound(availableSchedule.Error);
    }

    public static async Task<Results<Ok, NotFound<string>>> BookSlot(SlotRequest slotRequest,
        ISlotAvailabilityService slotClientAvailabilityService)
    {
        var validator = new SlotRequestValidator();
        var validationResult = await validator.ValidateAsync(slotRequest);

        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            return TypedResults.NotFound($"Invalid slot request: {errors}");
        }

        var bookSlotResult = await slotClientAvailabilityService.BookSlot(slotRequest);

        return bookSlotResult.IsSuccess
            ? TypedResults.Ok()
            : TypedResults.NotFound("Unable to book slot");
    }
}