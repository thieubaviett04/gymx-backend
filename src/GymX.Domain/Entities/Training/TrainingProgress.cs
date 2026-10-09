using GymX.Domain.Common.Models;
using GymX.Domain.Constants;

namespace GymX.Domain.Entities.Training;

public class TrainingProgress : EntityAuditBase
{
    public Guid MemberId { get; private set; }
    public Guid TrainerId { get; private set; }
    public Guid? TrainingSessionId { get; private set; }
    public DateOnly RecordDate { get; private set; }
    public string? ResultNotes { get; private set; }
    public string? AdjustmentPlan { get; private set; }
    public decimal? Weight { get; private set; }
    public decimal? BodyFatPercentage { get; private set; }

    protected TrainingProgress() { }

    public static TrainingProgress Create(Guid memberId, Guid trainerId, DateOnly recordDate, Guid? trainingSessionId = null, string? resultNotes = null, string? adjustmentPlan = null, decimal? weight = null, decimal? bodyFatPercentage = null)
    {
        return new TrainingProgress
        {
            MemberId = memberId,
            TrainerId = trainerId,
            TrainingSessionId = trainingSessionId,
            RecordDate = recordDate,
            ResultNotes = resultNotes,
            AdjustmentPlan = adjustmentPlan,
            Weight = weight,
            BodyFatPercentage = bodyFatPercentage
        };
    }
}
