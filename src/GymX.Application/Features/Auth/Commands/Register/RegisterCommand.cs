using FluentValidation;
using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Authencation;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using GymX.Domain.Constants;
using GymX.Domain.Entities.Identity;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace GymX.Application.Features.Auth.Commands.Register;

// Request DTO (Command)
public record RegisterCommand(
    string FullName,
    string Email,
    string PhoneNumber,
    string Password) : IRequest<Result<RegisterResponse>>;

// Response DTO
public record RegisterResponse
{
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}

// Cấu trúc dùng để đóng gói và ném vào Cache (Limbo State)
public record PendingRegistrationCacheItem(
    string FullName,
    string Email,
    string PhoneNumber,
    string PasswordHash,
    string OtpCode);


