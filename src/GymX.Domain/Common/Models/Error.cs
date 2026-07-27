

namespace GymX.Domain.Common.Models
{
    public record Error(
        string Code,
        string Description,
        ErrorType Type = ErrorType.Failure,
        string? MessageKey = null)
    {
        /// <summary>Key i18n tự động dùng Code nếu MessageKey không truyền vào.</summary>
        public string ResolvedMessageKey => MessageKey ?? Code;
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
        public static readonly Error NullValue = new("error.null", "Null value was provided", ErrorType.Validation);
        public static Error NotFound(string code, string desc, string? messageKey = null) =>
            new(code, desc, ErrorType.NotFound, messageKey);
        public static Error Conflict(string code, string desc, string? messageKey = null) =>
            new(code, desc, ErrorType.Conflict, messageKey);
        public static Error Validation(string code, string desc, string? messageKey = null) =>
            new(code, desc, ErrorType.Validation, messageKey);
        public static Error Unauthorized(string code, string desc, string? messageKey = null) =>
            new(code, desc, ErrorType.Unauthorized, messageKey);
        public static Error Forbidden(string code, string desc, string? messageKey = null) =>
            new(code, desc, ErrorType.Forbidden, messageKey);
        public static Error Failure(string code, string desc, string? messageKey = null) =>
            new(code, desc, ErrorType.Failure, messageKey);
    }
    public enum ErrorType
    {
        Failure,
        Validation,
        NotFound,
        Conflict,
        Unauthorized,
        Forbidden
    }
}
