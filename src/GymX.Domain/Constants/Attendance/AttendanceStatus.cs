namespace GymX.Domain.Constants;

public static class AttendanceStatus
{
    public const string Present = "PRESENT";
    public const string Late = "LATE";
    public const string EarlyLeave = "EARLY_LEAVE";
    public const string LateAndEarlyLeave = "LATE_AND_EARLY_LEAVE";
    public const string Absent = "ABSENT";
    public const string MissingCheckIn = "MISSING_CHECK_IN";
    public const string MissingCheckOut = "MISSING_CHECK_OUT";
}
