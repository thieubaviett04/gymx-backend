namespace GymX.Application.Modules.Violations.DTOs.Response;

public record EmployeeLookupDto(
    Guid Id,
    string EmployeeCode,
    string FullName,
    string Position,
    string? PhoneNumber,
    bool HasShiftToday);

