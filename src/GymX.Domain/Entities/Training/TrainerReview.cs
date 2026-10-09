using GymX.Domain.Common.Models;
using GymX.Domain.Constants;

namespace GymX.Domain.Entities.Training;

public class TrainerReview : EntityAuditBase
{
    public Guid TrainingSessionId { get; private set; }
    public Guid MemberId { get; private set; }
    public Guid TrainerId { get; private set; }
    public short Rating { get; private set; }
    public string? Comment { get; private set; }

    protected TrainerReview() { }

    public static TrainerReview Create(Guid trainingSessionId, Guid memberId, Guid trainerId, short rating, string? comment = null)
    {
        return new TrainerReview
        {
            TrainingSessionId = trainingSessionId,
            MemberId = memberId,
            TrainerId = trainerId,
            Rating = rating,
            Comment = comment
        };
    }
}
