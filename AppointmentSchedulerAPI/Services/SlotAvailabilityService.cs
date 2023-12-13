using System.Globalization;
using AppointmentSchedulerAPI.Contracts;
using AppointmentSchedulerAPI.Models;
using CSharpFunctionalExtensions;

namespace AppointmentSchedulerAPI.Services;

public class SlotAvailabilityService : ISlotAvailabilityService
{
    private readonly SlotClientService _slotClientService;

    public SlotAvailabilityService(SlotClientService slotClientService)
    {
        _slotClientService = slotClientService ?? throw new ArgumentNullException(nameof(slotClientService));
    }

    public async Task<Result<AvailableDoctorSchedule>> GetAvailableSchedule(string date)
    {
        var busyScheduleResult = await _slotClientService.GetBusySchedule(date);

        if (busyScheduleResult.IsFailure)
        {
            return Result.Failure<AvailableDoctorSchedule>(busyScheduleResult.Error);
        }

        var busySchedule = busyScheduleResult.Value;

        // Create a new AvailableDoctorSchedule object
        var availableSchedule = new AvailableDoctorSchedule
        {
            Facility = busySchedule.Facility,
            SlotDurationMinutes = busySchedule.SlotDurationMinutes,
            Days = new Dictionary<DayOfWeek, AvailablePeriod?>()
        };
        var inputDate = ParseDate(date);

        // Calculate the available slots for each day
        foreach (var day in busySchedule.Days)
        {
            if (day.Value != null)
            {
                var availableSlotsResult = CalculateAvailableSlots(day.Value.WorkPeriod,
                    busySchedule.SlotDurationMinutes,
                    day.Value.BusySlots, inputDate);

                var availablePeriod = new AvailablePeriod
                {
                    WorkPeriod = day.Value.WorkPeriod,
                    AvailableSlots = availableSlotsResult.Value
                };
                availableSchedule.Days[day.Key] = availablePeriod;
            }

            inputDate = inputDate.AddDays(1);
        }

        return Result.Success(availableSchedule);
    }

    public Result<List<Slot>> CalculateAvailableSlots(WorkPeriod? workPeriod, int slotDurationMinutes,
        List<Slot>? busySlots,
        DateTime currentDateTime)
    {
        var workDayEnd = TimeSpan.FromHours(workPeriod.EndHour);
        var lunchStart = TimeSpan.FromHours(workPeriod.LunchStartHour);
        var lunchEnd = TimeSpan.FromHours(workPeriod.LunchEndHour);

        var availableSlots = new List<TimeSpan>();
        var currentSlot = TimeSpan.FromHours(workPeriod.StartHour);

        while (currentSlot < workDayEnd)
        {
            if (currentSlot == lunchStart)
            {
                currentSlot = lunchEnd;
            }

            if (busySlots.Any(x => x.Start.TimeOfDay == currentSlot))
            {
                currentSlot = currentSlot.Add(TimeSpan.FromMinutes(slotDurationMinutes));
                continue;
            }

            availableSlots.Add(currentSlot);
            currentSlot = currentSlot.Add(TimeSpan.FromMinutes(slotDurationMinutes));
        }

        var slots = availableSlots
            .Select(x => Slot.FromTimeSpan(x, slotDurationMinutes, currentDateTime))
            .ToList();

        return Result.Success(slots);
    }

    public async Task<Result> BookSlot(SlotRequest slotRequest)
    {
        if (slotRequest.Start is null || slotRequest.End is null)
        {
            return Result.Failure("Invalid slot request");
        }

        var slotStart = ParseDateTime(slotRequest.Start);
        var slotEnd = ParseDateTime(slotRequest.End);

        var date = slotStart.ToString(Constants.DateFormat);
        var availableScheduleResult = await GetAvailableSchedule(date);

        if (availableScheduleResult.IsFailure)
        {
            return Result.Failure(availableScheduleResult.Error);
        }

        var availableSchedule = availableScheduleResult.Value;

        if (!IsSlotAvailable(slotStart, slotEnd, availableSchedule))
        {
            return Result.Failure("Slot is not available");
        }

        var request = CreateSlotRequest(slotStart, slotEnd, slotRequest, availableSchedule);
        var bookSlotResult = await _slotClientService.PostBookSlotRequest(request);

        return bookSlotResult.IsSuccess ? Result.Success() : Result.Failure("Failed to book slot");
    }

    private DateTimeOffset ParseDateTime(string dateTimeString)
    {
        string[] formats = { Constants.DateTimeFormatWithSpace, Constants.DateTimeFormatWithT };
        if (!DateTimeOffset.TryParseExact(dateTimeString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var dateTime))
        {
            throw new FormatException("Invalid date format");
        }

        return dateTime;
    }

    private bool IsSlotAvailable(DateTimeOffset slotStart, DateTimeOffset slotEnd,
        AvailableDoctorSchedule availableSchedule)
    {
        var availableSlots = availableSchedule.Days[slotStart.DayOfWeek]?.AvailableSlots;
        return availableSlots is not null && availableSlots
            .Any(slot => slot.Start <= slotStart && slot.End >= slotEnd);
    }

    private SlotRequest CreateSlotRequest(DateTimeOffset slotStart, DateTimeOffset slotEnd,
        SlotRequest slotRequest, AvailableDoctorSchedule availableSchedule)
    {
        return new SlotRequest
        {
            FacilityId = availableSchedule?.Facility?.FacilityId,
            Start = slotStart.ToString(Constants.DateTimeFormatWithSpace),
            End = slotEnd.ToString(Constants.DateTimeFormatWithSpace),
            Comments = slotRequest.Comments,
            Patient = slotRequest.Patient
        };
    }

    private DateTime ParseDate(string date)
        => DateTime.ParseExact(date, Constants.DateFormat, CultureInfo.InvariantCulture);
}