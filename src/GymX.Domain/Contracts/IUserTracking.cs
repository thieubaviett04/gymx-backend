namespace GymX.Domain.Contracts;

public interface IUserTracking
{
    string? CreatedBy { get; set; }
    string? LastModifiedBy { get; set; }
}
