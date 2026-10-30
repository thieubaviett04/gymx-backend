using System.Security.Cryptography;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Contracts;

namespace GymX.Domain.Entities.Packages;

public class Package : EntityAuditBase, ISoftDelete
{
    public string PackageCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int DurationMonths { get; private set; }
    public int DurationDays { get; private set; }
    public int? MaxCheckins { get; private set; }
    public string? Description { get; private set; }
    public string Status { get; private set; } = PackageStatus.Active;
    public DateOnly EffectiveStartDate { get; private set; }
    public DateOnly? EffectiveEndDate { get; private set; }

    public Guid? PreviousPackageId { get; private set; }
    public virtual Package? PreviousPackage { get; private set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }

    protected Package() { }

    public static string GenerateRandomCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var buffer = new char[6];
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[6];
        rng.GetBytes(bytes);

        for (int i = 0; i < 6; i++)
        {
            buffer[i] = chars[bytes[i] % chars.Length];
        }

        return new string(buffer);
    }

    public static Package Create(
        string name,
        decimal price,
        int durationMonths,
        DateOnly effectiveStartDate,
        DateOnly? effectiveEndDate = null,
        int? maxCheckins = null,
        string? description = null,
        string? customCode = null,
        Guid? previousPackageId = null)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var initialStatus = effectiveStartDate > today ? PackageStatus.Inactive : PackageStatus.Active;

        return new Package
        {
            PackageCode = string.IsNullOrWhiteSpace(customCode) ? GenerateRandomCode() : customCode.Trim().ToUpperInvariant(),
            Name = name.Trim(),
            Price = price,
            DurationMonths = durationMonths,
            DurationDays = durationMonths * 30,
            MaxCheckins = maxCheckins,
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            EffectiveStartDate = effectiveStartDate,
            EffectiveEndDate = effectiveEndDate,
            Status = initialStatus,
            PreviousPackageId = previousPackageId
        };
    }

    public void RegenerateCode(string newCode)
    {
        PackageCode = newCode.Trim().ToUpperInvariant();
    }

    public void UpdateInfo(
        string name,
        decimal price,
        int durationMonths,
        DateOnly effectiveStartDate,
        DateOnly? effectiveEndDate,
        string? status,
        int? maxCheckins = null,
        string? description = null)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        Name = name.Trim();
        Price = price;
        DurationMonths = durationMonths;
        DurationDays = durationMonths * 30;
        MaxCheckins = maxCheckins;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        EffectiveStartDate = effectiveStartDate;
        EffectiveEndDate = effectiveEndDate;

        if (effectiveStartDate > today)
        {
            Status = PackageStatus.Inactive;
        }
        else if (!string.IsNullOrWhiteSpace(status) && PackageStatus.IsValid(status))
        {
            Status = status.Trim().ToUpperInvariant();
        }
    }

    public void Deactivate()
    {
        Status = PackageStatus.Inactive;
    }

    public void Activate()
    {
        Status = PackageStatus.Active;
    }

    public void Discontinue(DateOnly? endDate = null)
    {
        Status = PackageStatus.Discontinued;
        EffectiveEndDate = endDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedDate = DateTime.UtcNow;
    }
}
