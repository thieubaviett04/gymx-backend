using FluentValidation;
using GymX.Application.Common.Interfaces;
using GymX.Application.Common.Interfaces.Repositories;
using GymX.Domain.Common.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace GymX.Application.Features.Auth.Commands.VerifyEmail;

// Request DTO
public record VerifyEmailCommand(string Email, string OtpCode) : IRequest<Result<string>>;


