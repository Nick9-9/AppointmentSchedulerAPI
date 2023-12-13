using System.Net.Http.Headers;
using System.Text;
using AppointmentSchedulerAPI.Contracts;
using AppointmentSchedulerAPI.Services;
using Microsoft.OpenApi.Models;

namespace AppointmentSchedulerAPI.Middleware;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISlotAvailabilityService, SlotAvailabilityService>();
        return services;
    }

    public static IServiceCollection AddHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<SlotClientService>((serviceProvider, httpClient) =>
        {
            var byteArray = Encoding.ASCII.GetBytes("techuser:secretpassWord");
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            httpClient.BaseAddress = new Uri("https://draliatest.azurewebsites.net");
        });
        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1",
                new OpenApiInfo { Title = "Appointment Scheduler API", Version = "v1", Description = "20231120" });
        });
        return services;
    }
}