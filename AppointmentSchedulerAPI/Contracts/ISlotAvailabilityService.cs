using AppointmentSchedulerAPI.Models;
using CSharpFunctionalExtensions;

namespace AppointmentSchedulerAPI.Contracts;

public interface ISlotAvailabilityService
{
    Task<Result<AvailableDoctorSchedule>> GetAvailableSchedule(string date);

    Result<List<Slot>> CalculateAvailableSlots(WorkPeriod? workPeriod, int slotDurationMinutes, List<Slot>? busySlots,
        DateTime currentDateTime);

    Task<Result> BookSlot(SlotRequest slotRequest);
}