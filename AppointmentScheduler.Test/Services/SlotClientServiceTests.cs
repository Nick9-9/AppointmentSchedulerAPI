using System.Net;
using RichardSzalay.MockHttp;
using System.Text.Json;
using AppointmentSchedulerAPI.Services;
using AppointmentSchedulerAPI.Models;

namespace AppointmentScheduler.Test.Services
{
    public class SlotClientServiceTests
    {
        private readonly SlotClientService  _slotClientService;
        private readonly MockHttpMessageHandler _mockHttp;

        public SlotClientServiceTests()
        {
            _mockHttp = new MockHttpMessageHandler();
            var httpClient = _mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("https://draliatest.azurewebsites.net");
            _slotClientService = new SlotClientService(httpClient);
        }

        [Fact]
        public async Task GetBusySchedule_ShouldReturnBusyDoctorSchedule_WhenApiCallIsSuccessful()
        {
            // Arrange
            var date = "20220101";
            var expectedBusySchedule = new BusyDoctorSchedule();
            _mockHttp.When($"*api/availability/GetWeeklyAvailability/{date}")
                .Respond("application/json", JsonSerializer.Serialize(expectedBusySchedule));

            // Act
            var result = await _slotClientService.GetBusySchedule(date);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(expectedBusySchedule);
        }
        
        [Fact]
        public async Task PostBookSlotRequest_ShouldReturnSuccess_WhenApiCallIsSuccessful()
        {
            // Arrange
            var slotRequest = new SlotRequest();
            _mockHttp.When("*api/availability/TakeSlot")
                .Respond(HttpStatusCode.OK);

            // Act
            var result = await _slotClientService.PostBookSlotRequest(slotRequest);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
        
        [Fact]
        public async Task PostBookSlotRequest_ShouldReturnFailure_WhenApiCallFails()
        {
            // Arrange
            var slotRequest = new SlotRequest();
            _mockHttp.When("*api/availability/TakeSlot")
                .Respond(HttpStatusCode.InternalServerError);
            // Act
            var result = await _slotClientService.PostBookSlotRequest(slotRequest);

            // Assert
            result.IsFailure.Should().BeTrue();
        }
        
        [Fact]
        public async Task PostBookSlotRequest_ShouldReturnFailure_WhenApiCallReturnsBadRequest()
        {
            // Arrange
            var slotRequest = new SlotRequest();
            _mockHttp.When("*api/availability/TakeSlot")
                .Respond(HttpStatusCode.BadRequest);

            // Act
            var result = await _slotClientService.PostBookSlotRequest(slotRequest);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Should().Be("Unable to book a slot");
        }
    }
}