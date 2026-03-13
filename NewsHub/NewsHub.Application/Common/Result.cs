using System;

namespace NewsHub.Application.Common
{
    public class Result<T>
    {
        public bool Success { get; }
        public bool IsFailure => !Success;

        public string? Error { get; }
        public string? Message { get; }
        public T? Data { get; }

        public TypeMessage TypeMessage { get; }

        private Result(bool success, T? data, string? error, string? message, TypeMessage typeMessage)
        {
            Success = success;
            Data = data;
            Error = error;
            Message = message;
            TypeMessage = typeMessage;
        }

        public static Result<T> Ok(T data, string? message = null)
            => new Result<T>(true, data, null, message, TypeMessage.Success);

        public static Result<T> Fail(string error, TypeMessage type, string? message = null)
            => new Result<T>(false, default, error, message, type);
    }
}