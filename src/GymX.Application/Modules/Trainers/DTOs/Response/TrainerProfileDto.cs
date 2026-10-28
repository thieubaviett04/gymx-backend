namespace GymX.Application.Modules.Trainers.DTOs.Response;

public class TrainerProfileDto
{
    public Guid Id { get; set; }
    public string SpecializationName { get; set; } = string.Empty;
    public short ExperienceYears { get; set; }
    public string? Description { get; set; }
    public decimal RatingAvg { get; set; }
    public int RatingCount { get; set; }
    public List<TrainerDocumentDto> Documents { get; set; } = [];
}

public class TrainerDocumentDto
{
    public Guid Id { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? IssuingOrganization { get; set; }
    public string? DocumentNumber { get; set; }
    public DateOnly? IssuedDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public string VerificationStatus { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
}
