

namespace GymX.Domain.Common.Models
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }
        protected Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None)
                throw new InvalidOperationException("Success result cannot contain an error.");
            if (!isSuccess && error == Error.None)
                throw new InvalidOperationException("Failure result must contain an error.");
            IsSuccess = isSuccess;
            Error = error;
        }
        public static Result Success() => new(true, Error.None);
        public static Result Failure(Error error) => new(false, error);
        public static Result<TValue> Success<TValue>(TValue value) => Result<TValue>.Success(value);
        public static Result<TValue> Failure<TValue>(Error error) => Result<TValue>.Failure(error);
    }
    public class Result<TValue> : Result
    {
        private readonly TValue? _value;
        protected Result(TValue? value, bool isSuccess, Error error)
            : base(isSuccess, error)
        {
            _value = value;
        }
        public TValue Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("The value of a failure result cannot be accessed.");
        public static Result<TValue> Success(TValue value) => new(value, true, Error.None);
        public static new Result<TValue> Failure(Error error) => new(default, false, error);
        public static implicit operator Result<TValue>(TValue? value) =>
            value is not null ? Success(value) : Failure(Error.NullValue);
    }
}
