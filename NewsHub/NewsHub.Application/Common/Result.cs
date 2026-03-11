using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.Common
{
    public class Result<T>
    {
        public bool Success { get; }
        public bool IsFailure => !Success;
        public string? Error { get; }
        public T? Data { get; }
        public TypeMessage TypeMessage { get; }

        private Result(bool success, T? data, string? error, TypeMessage typeMessage)
        {
            Success = success;
            Data = data;
            Error = error;
            TypeMessage = typeMessage;
        }

        public static Result<T> Ok(T data)
            => new Result<T>(true, data, null, TypeMessage.Success);

        public static Result<T> Fail(string error, TypeMessage type)
            => new Result<T>(false, default, error, type);
    }
}
