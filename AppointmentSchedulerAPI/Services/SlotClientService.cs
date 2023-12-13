using System.Text;
using System.Text.Json;
using AppointmentSchedulerAPI.Models;
using CSharpFunctionalExtensions;

namespace AppointmentSchedulerAPI.Services;

public class SlotClientService
{
    private readonly HttpClient _httpClient;

    private const string TakeSlotEndpoint = "/api/availability/TakeSlot";
    private const string GetWeeklyAvailabilityEndpoint = "/api/availability/GetWeeklyAvailability/{0}";

    public SlotClientService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<Result<BusyDoctorSchedule>> GetBusySchedule(string date)
    {
        var apiUrl = string.Format(GetWeeklyAvailabilityEndpoint, date);
        var busySchedule = await _httpClient.GetFromJsonAsync<BusyDoctorSchedule>(apiUrl);

        return busySchedule is not null
            ? Result.Success(busySchedule)
            : Result.Failure<BusyDoctorSchedule>("Unable to get availability");
    }

    public async Task<Result> PostBookSlotRequest(SlotRequest request)
    {
        var requestContent = JsonSerializer.Serialize(request);
        var content = new StringContent(requestContent, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(TakeSlotEndpoint, content);

        return response.IsSuccessStatusCode
            ? Result.Success()
            : Result.Failure("Unable to book a slot");
    }
}