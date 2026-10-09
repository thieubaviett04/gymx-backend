using GymX.Domain.Common.Models;
using GymX.Domain.Constants;

namespace GymX.Domain.Entities.Attendance;

public class ShiftDefinition : EntityAuditBase
{
    public string Name { get; private set; } = string.Empty;
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public string Status { get; private set; } = "ACTIVE";

    protected ShiftDefinition() { }

    public static ShiftDefinition Create(string name, TimeOnly startTime, TimeOnly endTime)
    {
        return new ShiftDefinition
        {
            Name = name,
            StartTime = startTime,
            EndTime = endTime,
            Status = "ACTIVE"
        };
    }

    public void Deactivate()
    {
        Status = "INACTIVE";
    }
}
