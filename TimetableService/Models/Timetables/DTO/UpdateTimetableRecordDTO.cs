using FluentValidation;

namespace TimetableService.Models.Timetables.DTO;

public record UpdateTimetableRecordDTO(int HospitalId, int DoctorId, DateTime From, DateTime To, string Room);

public class UpdateTimetableRecordDTOValidator : AbstractValidator<UpdateTimetableRecordDTO>
{
    public UpdateTimetableRecordDTOValidator()
    {
        RuleFor(i => i.From)
            .Must(i => i.Minute % 30 == 0 && i.Second == 0)
            .WithMessage("Количество минут должно быть кратно 30, а секунды всегда равны 0")
            .LessThanOrEqualTo(i => i.To.AddHours(-12))
            .WithMessage("Разница между временем начала приёма и окончания не может быть больше 12 часов");

        RuleFor(i => i.To)
            .Must(i => i.Minute % 30 == 0 && i.Second == 0)
            .WithMessage("Количество минут должно быть кратно 30, а секунды всегда равны 0")
            .GreaterThanOrEqualTo(i => i.From)
            .WithMessage("Время начала приёма не может быть больше времени окончания");
    }
}
