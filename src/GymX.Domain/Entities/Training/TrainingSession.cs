using GymX.Domain.Common.Models;
using GymX.Domain.Constants;

namespace GymX.Domain.Entities.Training;

public class TrainingSession : EntityAuditBase
{
    public Guid MemberId { get; private set; }
    public Guid TrainerId { get; private set; }
    public Guid TrainingServiceId { get; private set; }
    public DateTime StartAt { get; private set; }
    public DateTime EndAt { get; private set; }
    public decimal PriceAtBooking { get; private set; }
    public string Status { get; private set; } = TrainingSessionStatus.Booked;
    public string? CancellationReason { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public Guid? CompletedByTrainerId { get; private set; }

    protected TrainingSession() { }

    public static TrainingSession Create(Guid memberId, Guid trainerId, Guid trainingServiceId, DateTime startAt, DateTime endAt, decimal priceAtBooking)
    {
        return new TrainingSession
        {
            MemberId = memberId,
            TrainerId = trainerId,
            TrainingServiceId = trainingServiceId,
            StartAt = startAt,
            EndAt = endAt,
            PriceAtBooking = priceAtBooking,
            Status = TrainingSessionStatus.Booked
        };
    }

    public void Confirm()
    {
        Status = TrainingSessionStatus.Confirmed;
    }

    public void Complete(Guid trainerId)
    {
        Status = TrainingSessionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        CompletedByTrainerId = trainerId;
    }

    public void Cancel(string reason)
    {
        Status = TrainingSessionStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason;
    }
}
