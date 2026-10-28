namespace GymX.Application.Modules.Employees.DTOs;

public class EmployeeDetailDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? NationalId { get; set; }
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? AvatarUrl { get; set; }
    public string Position { get; set; } = string.Empty;
    public decimal BaseSalaryRate { get; set; }
    public DateOnly HiredAt { get; set; }
    public string EmploymentStatus { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedDate { get; set; }
    public TrainerDetailInfoDto? TrainerInfo { get; set; }
}

public class TrainerDetailInfoDto
{
    public Guid TrainerId { get; set; }
    public Guid SpecializationId { get; set; }
    public string SpecializationName { get; set; } = string.Empty;
    public short ExperienceYears { get; set; }
    public decimal? SalaryCoefficient { get; set; }
    public string? Description { get; set; }
    public decimal RatingAvg { get; set; }
    public int RatingCount { get; set; }
}
