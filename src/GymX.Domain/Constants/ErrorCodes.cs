
namespace GymX.Domain.Constants
{
    public static class ErrorCodes
    {
        public static class Common
        {
            public const string InternalServerError = "error.internal_server_error";
            public const string ValidationError = "validation.failed";
            public const string NotFound = "common.not_found";
            public const string Unauthorized = "auth.unauthorized";
            public const string Forbidden = "auth.forbidden";
        }
    }
}
