using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Application.Common
{
    public class Result<T>
    {
        public bool Success { get; }
        public string? Error { get; }
        public T? Data { get; }

        public Result(bool success, T? data, string? error)
        {
            Success = success;
            Data = data;
            Error = error;
        }

        public static Result<T> Ok(T data) => new Result<T>(true, data, null);

        public static Result<T> Fail(string error) => new Result<T>(false, default, error);
    }
}
