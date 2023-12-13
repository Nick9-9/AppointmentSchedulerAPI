using System.Text.RegularExpressions;
using AppointmentSchedulerAPI.Models;
using FluentValidation;

namespace AppointmentSchedulerAPI.Validation;

public class SlotRequestValidator : AbstractValidator<SlotRequest>
{
    public SlotRequestValidator()
    {
        RuleFor(x => x.Start).NotEmpty().WithMessage("Start time is required.");
        RuleFor(x => x.End).NotEmpty().WithMessage("End time is required.");
        RuleFor(x => x.Patient).NotNull().WithMessage("Patient details are required.");
        RuleFor(x => x.FacilityId).NotEmpty().WithMessage("Facility ID is required.");
    }
}