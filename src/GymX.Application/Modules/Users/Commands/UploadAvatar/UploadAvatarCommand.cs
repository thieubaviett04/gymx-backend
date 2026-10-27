using GymX.Domain.Common.Models;
using MediatR;

namespace GymX.Application.Modules.Users.Commands.UploadAvatar;

public record UploadAvatarCommand(
    Stream FileStream,
    string FileName,
    string ContentType,
    long Length
) : IRequest<Result<string>>;
