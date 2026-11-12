namespace GymX.Application.Modules.Violations.DTOs.Request;

public class CancelViolationRequest
{
    public string? CancellationReason { get; set; }
    public string? Reason { get; set; }

    public string GetEffectiveReason()
    {
        if (!string.IsNullOrWhiteSpace(CancellationReason))
        {
            return CancellationReason.Trim();
        }

        if (!string.IsNullOrWhiteSpace(Reason))
        {
            return Reason.Trim();
        }

        return "Hủy bởi quản trị viên";
    }
}
